using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAML.Parser;
using AMF.Tools.Core.ClientGenerator;
using AMF.Tools.Core.WebApiGenerator;
using AMF.Tools.Core;
using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using CustomHost;
using EnvDTE;



namespace RamlTest_Jan28
{
    class Program
    {
        static void Main(string[] args)
        {
            Program sa = new Program();

            System.Threading.Tasks.Task.Run(async () => await sa.ShouldBuild_WhenCustomScalar());
            ClientGeneratorModel amfmodelResult = sa.ShouldBuild_WhenCustomScalar().Result;
            if (amfmodelResult != null)
            {
                sa.ProcessTemplate(amfmodelResult);
            }

            //Task.Run(async () => await sa.ShouldWorkIncludeWithRelativeIncludes());
            //WebApiGeneratorModel amfmodelResult1 = sa.ShouldWorkIncludeWithRelativeIncludes().Result;

        }


        public async Task<ClientGeneratorModel> ShouldBuild_WhenCustomScalar()
        {
            var model = await GetCustomScalarModel();
            return model;


        }
        private async Task<ClientGeneratorModel> GetCustomScalarModel()
        {
            return await BuildModel("files/relative-include.raml");
        }
        private async Task<ClientGeneratorModel> BuildModel(string ramlFile)
        {
            
            var fi = new FileInfo(ramlFile);
            var raml = await new RamlParser().Load(fi.FullName);
            var model = new ClientGeneratorService(raml, "test", "TargetNamespace", "TargetNamespace.Models").BuildModel();

            return model;
        }
        
        public async Task<WebApiGeneratorModel> ShouldWorkIncludeWithRelativeIncludes()
        {
            var raml = await new RamlParser().Load("files/relative-include.raml");
            var model = new WebApiGeneratorService(raml, "TestNs", "TestNs.Models").BuildModel();
            //Assert.IsNotNull(model);
            return model;
        }
        public void ProcessTemplate(ClientGeneratorModel model)
        {
            string templatePath = @"E:\RAML Code Generation\raml-dotnet-tools-master\src\tools\RAML.Tools\Templates\Client\RAMLClient.t4";
                   CustomCmdLineHost host = new CustomCmdLineHost();
            Engine engine = new Engine();
            host.TemplateFileValue = model.ToString();
            var dte = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as DTE;

            //Read the text template.
            string input = File.ReadAllText(templatePath);
            //Transform the text template.
            string output = engine.ProcessTemplate(input, host);
            string outputFileName = Path.GetFileNameWithoutExtension(templatePath);
            outputFileName = Path.Combine(Path.GetDirectoryName(templatePath), outputFileName);
            outputFileName = outputFileName + "1" + host.FileExtension;
            File.WriteAllText(outputFileName, output, host.FileEncoding);
        }
    }
}
