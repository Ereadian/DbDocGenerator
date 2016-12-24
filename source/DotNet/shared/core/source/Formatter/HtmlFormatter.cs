//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="HtmlFormater.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Formater
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Web.Razor;
    using System.CodeDom.Compiler;
    using System.Reflection;
    using Microsoft.CSharp;

    public class HtmlFormatter : IFormatter
    {
        
        private readonly Lazy<RazorTemplateBase<IDatabaseAnalysisResult>> engineLoader;

        public HtmlFormatter()
        {
            engineLoader = new Lazy<RazorTemplateBase<IDatabaseAnalysisResult>>(
                ()=>
                {
                    var template = LoadTemplate("main.cshtml");
                    var className = "TemplateRendering" + Guid.NewGuid().ToString("N");
                    var assembly = Compile(typeof(IDatabaseAnalysisResult), className, template);
                    var instance = assembly.CreateInstance(typeof(IFormatter).Namespace + "." + className);
                    return instance as RazorTemplateBase<IDatabaseAnalysisResult>;
                });
        }

        /// <summary>
        /// Format output
        /// </summary>
        /// <param name="title">document title</param>
        /// <param name="analysisResult">analysis result</param>
        /// <param name="outputSteram">output stream</param>
        public void Format(string title, IDatabaseAnalysisResult analysisResult, Stream outputSteram)
        {
            analysisResult.Title = title;
            var engine = engineLoader.Value;
            engine.Model = analysisResult;
            engine.Execute();
            using (var writer = new StreamWriter(outputSteram))
            {
                writer.Write(engine.Buffer.ToString());
            }
            engine.Buffer.Clear();
        }

        /// <summary>
        /// load content from assembly
        /// </summary>
        /// <param name="templateName">template file name</param>
        /// <returns>template content</returns>
        private static string LoadTemplate(string templateName)
        {
            string content;
            var type = typeof(HtmlFormatter);
            using (var stearm = type.Assembly.GetManifestResourceStream(type, templateName))
            {
                using (var reader = new StreamReader(stearm))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }

        private static GeneratorResults GenerateCode(Type modelType, string className, string template)
        {
            var templateType = typeof(RazorTemplateBase<>).MakeGenericType(modelType);
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = templateType.AssemblyQualifiedName;
            host.DefaultNamespace = typeof(IFormatter).Namespace;
            host.DefaultClassName = className;
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Collections.Generic");
            host.NamespaceImports.Add("System.Linq");
            GeneratorResults razorResult;
            using (TextReader reader = new StringReader(template))
            {
                razorResult = new RazorTemplateEngine(host).GenerateCode(reader);
            }

            return razorResult;
        }

        private static CompilerParameters BuildCompilerParameters()
        {
            var @params = new CompilerParameters();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.ManifestModule.Name != "<In Memory Module>")
                    @params.ReferencedAssemblies.Add(assembly.Location);
            }

            @params.GenerateInMemory = true;
            @params.IncludeDebugInformation = false;
            @params.GenerateExecutable = false;
            @params.CompilerOptions = "/target:library /optimize";
            return @params;
        }

        private static Assembly Compile(Type modelType, string className, string template)
        {
            var builder = new StringBuilder();
            var codeProvider = new CSharpCodeProvider();
            using (var writer = new StringWriter(builder))
            {
                var generatorResults = GenerateCode(modelType, className, template);
                codeProvider.GenerateCodeFromCompileUnit(generatorResults.GeneratedCode, writer, new CodeGeneratorOptions());
            }

            var result = codeProvider.CompileAssemblyFromSource(BuildCompilerParameters(), new[] { builder.ToString() });
            if (result.Errors != null && result.Errors.HasErrors)
            {
                var errorBuilder = new StringBuilder(builder.ToString());
                errorBuilder.AppendLine();
                errorBuilder.AppendLine("Errors:");
                for (var i = 0; i < result.Errors.Count; i++)
                {
                    errorBuilder.AppendLine(result.Errors[i].ToString());
                }
                throw new ApplicationException(errorBuilder.ToString());
            }

            return result.CompiledAssembly;
        }
    }
}
