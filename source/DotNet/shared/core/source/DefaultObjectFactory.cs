//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DefaultObjectFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    /// <summary>
    /// Object factory default implementation
    /// </summary>
    /// <typeparam name="TBaseClass">type of base class</typeparam>
    /// <typeparam name="TInterface">Type of the object to create. It should be an interface</typeparam>
    public class DefaultObjectFactory<TBaseClass, TInterface> : IObjectFactory<TBaseClass, TInterface>
        where TBaseClass : class
        where TInterface : class
    {
        /// <summary>
        /// Constructor information
        /// </summary>
        private static readonly ConstructorInfo TypeConstructorInformation;

        /// <summary>
        /// Initializes a static instance of the <see cref="DefaultObjectFactory{TBaseClass,TInterface}" /> class.
        /// </summary>
        static DefaultObjectFactory()
        {
            try
            {
                TypeConstructorInformation = CreateObjectConstructor(typeof(TBaseClass), typeof(TInterface));
            }
            catch (Exception exception)
            {
                TypeConstructorInformation = null;
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to create constructor while implementing interface {0}. Exception:{1}",
                    typeof(TInterface).FullName,
                    exception.ToString());
                Trace.TraceError(errorMessage);
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns>new instance which supports the interface</returns>
        public virtual TInterface Create()
        {
            return TypeConstructorInformation == null ? null : (TInterface)TypeConstructorInformation.Invoke(null);
        }

        /// <summary>
        /// Create a unique name
        /// </summary>
        /// <param name="prefix">name prefix</param>
        /// <returns>random unique name</returns>
        protected static string CreateName(string prefix)
        {
            var builder = new StringBuilder();
            foreach (var ch in prefix)
            {
                if (Char.IsLetterOrDigit(ch) || (ch == '_'))
                {
                    builder.Append(ch);
                }
            }

            builder.Append(Guid.NewGuid().ToString("N"));
            return builder.ToString();
        }

        /// <summary>
        /// Create assembly builder
        /// </summary>
        /// <param name="interfaceType">type of interface to implement</param>
        /// <returns>assembly builder</returns>
        protected static AssemblyBuilder CreateAssemblyBuilder(Type interfaceType)
        {
            var currentApplicationDomain = AppDomain.CurrentDomain;
            var dynamicAssemblyName = CreateName("AssemblyFor" + interfaceType.Name);
            return currentApplicationDomain.DefineDynamicAssembly(
                new AssemblyName(dynamicAssemblyName),
                AssemblyBuilderAccess.Run);
        }

        /// <summary>
        /// Create module builder
        /// </summary>
        /// <param name="assemblyBuilder">assembly builder</param>
        /// <param name="interfaceType">type of interface to implement</param>
        /// <returns>module builder</returns>
        protected static ModuleBuilder CreateModuleBuilder(AssemblyBuilder assemblyBuilder, Type interfaceType)
        {
            var moduleName = CreateName("ModuleFor" + interfaceType.Name);
            return assemblyBuilder.DefineDynamicModule(moduleName, true);
        }

        /// <summary>
        /// Create type builder
        /// </summary>
        /// <param name="moduleBuilder">module builder</param>
        /// <param name="baseType">base type</param>
        /// <param name="interfaceType">type of interface to implement</param>
        /// <returns>type builder</returns>
        protected static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, Type baseType, Type interfaceType)
        {
            var typeName = CreateName("ClassFor" + interfaceType.Name);
            return moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Class,
                baseType,
                new Type[] { interfaceType });
        }

        /// <summary>
        /// Create default constructor
        /// </summary>
        /// <param name="moduleBuilder">module builder</param>
        /// <param name="baseType">base type</param>
        /// <param name="interfaceType">type of interface to implement</param>
        /// <returns>default constructor builder</returns>
        protected static ConstructorBuilder CreateDefaultConstructor(TypeBuilder typeBuilder, Type baseType, Type interfaceType)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);
            var baseConstructorInformation = baseType.GetConstructor(Type.EmptyTypes);
            if (baseConstructorInformation == null)
            {
                throw new ArgumentException($"Could not found default constructor in type {baseType.FullName}, or the default constructor is not public");
            }

            var ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseConstructorInformation);
            ilGenerator.Emit(OpCodes.Ret);

            return constructorBuilder;
        }

        /// <summary>
        /// Create properties
        /// </summary>
        /// <param name="typeBuilder">type builder</param>
        /// <param name="interfaceType">type of the interface to implement</param>
        protected static void CreateProperties(TypeBuilder typeBuilder, Type interfaceType)
        {
            // build field for each property and implement these properties
            var propertyInformationCollection = interfaceType.GetProperties();
            if (!propertyInformationCollection.IsNullOrEmpty())
            {
                foreach (var propertyInformation in propertyInformationCollection)
                {
                    var name = propertyInformation.Name;
                    var fieldName = CreateName(name);
                    var fieldBuilder = typeBuilder.DefineField(
                        fieldName,
                        propertyInformation.PropertyType,
                        FieldAttributes.Private);
                    var propertyBuilder = typeBuilder.DefineProperty(
                        name,
                        PropertyAttributes.HasDefault,
                        propertyInformation.PropertyType,
                        Type.EmptyTypes);

                    if (propertyInformation.CanRead)
                    {
                        var getMethodReference = propertyInformation.GetGetMethod();
                        var getMethodBuilder = CreatePropertyMethod(typeBuilder, getMethodReference);
                        var getIlGenerator = getMethodBuilder.GetILGenerator();
                        getIlGenerator.Emit(OpCodes.Ldarg_0);
                        getIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                        getIlGenerator.Emit(OpCodes.Ret);
                        propertyBuilder.SetGetMethod(getMethodBuilder);
                    }

                    if (propertyInformation.CanWrite)
                    {
                        var setMethodReference = propertyInformation.GetSetMethod();
                        var setMethodBuilder = CreatePropertyMethod(typeBuilder, setMethodReference);
                        var setIlGenerator = setMethodBuilder.GetILGenerator();
                        setIlGenerator.Emit(OpCodes.Ldarg_0);
                        setIlGenerator.Emit(OpCodes.Ldarg_1);
                        setIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                        setIlGenerator.Emit(OpCodes.Ret);
                        propertyBuilder.SetSetMethod(setMethodBuilder);
                    }
                }
            }
        }

        /// <summary>
        /// Utility to create get/set method for property
        /// </summary>
        /// <param name="typeBuilder">type builder</param>
        /// <param name="methodReference">get/set method of the property from interface or base class </param>
        /// <returns>method builder for property get/set operation</returns>
        protected static MethodBuilder CreatePropertyMethod(TypeBuilder typeBuilder, MethodInfo methodReference)
        {
            var methodAttribute = MethodAttributes.Public
                | MethodAttributes.Virtual
                | MethodAttributes.SpecialName 
                | MethodAttributes.HideBySig;
            var parameterTypes = methodReference.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(
                methodReference.Name,
                methodAttribute,
                //CallingConventions.HasThis | CallingConventions.Standard,
                methodReference.ReturnType,
                parameterTypes);

            return methodBuilder;
        }

        /// <summary>
        /// Create type and return default constructor information
        /// </summary>
        /// <param name="baseType">base type</param>
        /// <param name="interfaceType">type of the interface to implement</param>
        /// <returns>constructor information</returns>
        protected static ConstructorInfo CreateObjectConstructor(Type baseType, Type interfaceType)
        {
            var assemblyBuilder = CreateAssemblyBuilder(interfaceType);
            var moduleBuilder = CreateModuleBuilder(assemblyBuilder, interfaceType);
            var typeBuilder = CreateTypeBuilder(moduleBuilder, baseType, interfaceType);
            var constructorBuilder = CreateDefaultConstructor(typeBuilder, baseType, interfaceType);
            CreateProperties(typeBuilder, interfaceType);
            var type = typeBuilder.CreateType();
            return type.GetConstructor(Type.EmptyTypes);
        } 
    }
}
