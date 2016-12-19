//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Utility.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class Utility
    {
        public static T CreateInstanceFromTypeName<T>(string typeName, ILogger logger) where T : class
        {
            T instance = null;
            try
            {
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    var type = Type.GetType(typeName);
                    if (type != null)
                    {
                        var rawData = Activator.CreateInstance(type);
                        if (rawData != null)
                        {
                            instance = (T)rawData;
                            if (instance == null)
                            {
                                logger.Write(Events.InstanceTypeMismatch, typeof(T).FullName, rawData.GetType().FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Write(Events.FailedToCreateInstanceFromConfiguration, typeof(T).FullName, exception);
            }

            return instance;
        }

        /// <summary>
        /// Create a unique name
        /// </summary>
        /// <param name="prefix">name prefix</param>
        /// <returns>random unique name</returns>
        public static string CreateName(string prefix)
        {
            var builder = new StringBuilder();
            foreach (var ch in prefix)
            {
                if (char.IsLetterOrDigit(ch) || (ch == '_'))
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
        public static AssemblyBuilder CreateAssemblyBuilder(Type interfaceType)
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
        public static ModuleBuilder CreateModuleBuilder(AssemblyBuilder assemblyBuilder, Type interfaceType)
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
        public static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, Type baseType, Type interfaceType)
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
        /// <param name="typeBuilder">type builder</param>
        /// <param name="baseType">base type</param>
        /// <param name="interfaceType">type of interface to implement</param>
        /// <returns>default constructor builder</returns>
        public static ConstructorBuilder CreateDefaultConstructor(TypeBuilder typeBuilder, Type baseType, Type interfaceType)
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

            var ctorIlGenerator = constructorBuilder.GetILGenerator();
            ctorIlGenerator.Emit(OpCodes.Ldarg_0);
            ctorIlGenerator.Emit(OpCodes.Call, baseConstructorInformation);
            ctorIlGenerator.Emit(OpCodes.Ret);

            return constructorBuilder;
        }

        /// <summary>
        /// Create properties
        /// </summary>
        /// <param name="typeBuilder">type builder</param>
        /// <param name="interfaceType">type of the interface to implement</param>
        public static void CreateProperties(TypeBuilder typeBuilder, Type interfaceType)
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
        public static MethodBuilder CreatePropertyMethod(TypeBuilder typeBuilder, MethodInfo methodReference)
        {
            var methodAttribute = MethodAttributes.Public
                | MethodAttributes.Virtual
                | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig;
            var parameterTypes = methodReference.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(
                methodReference.Name,
                methodAttribute,
                methodReference.ReturnType,
                parameterTypes);

            return methodBuilder;
        }
    }
}
