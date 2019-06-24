using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using AppGeneration.AppForGeneration;
using System.Xml.Linq;
using RazorLight;

namespace AppGeneration
{
    public static class AppGenerator
    {
        public static void generate(string modelFileName, string classForGeneration, string classTemplate, 
            string pathToDirectoryWithFeatures, string pathToTargetDirectory)
        {
//            GenerateModelXml(pathToDirectoryWithFeatures,  modelFileName);
            FeatureModel model = LoadModel(modelFileName);
            GenerateMainFile(model, classTemplate, classForGeneration, pathToDirectoryWithFeatures).Wait();
            CopyAppDirectoryToEnvironment(pathToDirectoryWithFeatures, pathToTargetDirectory);
        }
        
        public static void GenerateModelXml(string path, string generatedModelFileName)
        {
            List<FeatureWithParameters> featureListForModelGeneration = LoadFeaturesWithParams(path);
            GenerateXmlModel(featureListForModelGeneration, generatedModelFileName);
        }
        private static List<FeatureWithParameters> LoadFeaturesWithParams(string path)
        {
            List<FeatureWithParameters> featureListForModelGeneration = new List<FeatureWithParameters>();

            string[] filePaths = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories);
            string annotationPattern = @"\s*@AnnotationList.\w*";
            string patternFeature = @"\s*@AnnotationList.Feature\w*";
            string patternAbstract = @"\s*@AnnotationList.AbstractFeature\w*";
            string patternOnItemClickTO = @"\s*@AnnotationList.OnItemClickTO\w*";
            string patternOnItemClickFROM = @"\s*@AnnotationList.OnItemClickFROM\w*";
            string patternOnLongItemClickTO = @"\s*@AnnotationList.OnLongItemClickTO\w*";
            string patternOnLongItemClickFROM = @"\s*@AnnotationList.OnLongItemClickFROM\w*";
            
            foreach(var filePath in filePaths)
            {
                if (filePath.Contains("AnnotationList.java")) 
                    continue;
                
                using(var reader = new StreamReader(filePath))
                {
                    string featureName = null;
                    string abstractFeatureName = null;
                    string longClickTO = null;
                    string longClickFROM = null;
                    string clickTO = null;
                    string clickFROM = null;
                    
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        // check start of the new record lines
                        if (Regex.Match(line, annotationPattern).Success)
                        {
                            if (Regex.Match(line, patternOnItemClickTO).Success)
                                clickTO = line.Split("OnItemClickTO")[1].Split("\"")[1];

                            if (Regex.Match(line, patternOnItemClickFROM).Success)
                                clickFROM = line.Split("OnItemClickFROM")[1].Split("\"")[1];

                            if (Regex.Match(line, patternOnLongItemClickTO).Success)
                                longClickTO = line.Split("OnLongItemClickTO")[1].Split("\"")[1];

                            if (Regex.Match(line, patternOnLongItemClickFROM).Success)
                                longClickFROM = line.Split("OnLongItemClickFROM")[1].Split("\"")[1]; 
                           
                            if (Regex.Match(line, patternFeature).Success)
                                featureName = line.Split("Feature")[1].Split("\"")[1]; 

                            if (Regex.Match(line, patternAbstract).Success)
                                abstractFeatureName = line.Split("AbstractFeature")[1].Split("\"")[1];
                        }
                    }
                    if (featureName != null)
                        featureListForModelGeneration.Add(
                        new FeatureWithParameters(featureName, abstractFeatureName, longClickTO, longClickFROM, clickTO, clickFROM));
                }
            }

            return featureListForModelGeneration;
        }

        private static void GenerateXmlModel(List<FeatureWithParameters> featureListForModelGeneration, string generatedModelFileName)
        {
            XDocument xdoc = new XDocument();
            
            XElement model = new XElement("model");
            model.Add(new XAttribute("name", "generated_model"));

            var groupedByAbstractFeatures = featureListForModelGeneration.GroupBy(x => x.abstractFeatureName);
            foreach (var group in groupedByAbstractFeatures)
            {
                string abstractFeatureName = null;
                XElement concreteNodes = new XElement("concrete_nodes");
                foreach (var featureWithParams in group)
                {
                    if (abstractFeatureName == null)
                    {
                        abstractFeatureName = featureWithParams.abstractFeatureName;
                    }
                    
                    XElement concreteNode = new XElement("concrete_node");
                    concreteNode.Add(new XAttribute("name", featureWithParams.featureName));
                    concreteNode.Add(new XElement("selected", false));
                    
                    if (featureWithParams.clickTO != null)
                        concreteNode.Add(new XElement("clickTO", featureWithParams.clickTO));
                    
                    if (featureWithParams.clickFROM != null)
                        concreteNode.Add(new XElement("clickFROM", featureWithParams.clickFROM));
                    
                    if (featureWithParams.longClickTO != null)
                        concreteNode.Add(new XElement("longClickTO", featureWithParams.longClickTO));

                    if (featureWithParams.longClickFROM != null)
                        concreteNode.Add(new XElement("longClickFROM", featureWithParams.longClickFROM));
                    
                    concreteNodes.Add(concreteNode);
                }
                
                XElement abstractNode = new XElement("abstract_node");
                abstractNode.Add(new XAttribute("name", abstractFeatureName));
                abstractNode.Add(concreteNodes);
                
                model.Add(abstractNode);
            }
            
            xdoc.Add(model);
            xdoc.Save(generatedModelFileName);
        }
        
        private static async Task GenerateMainFile(FeatureModel model, string classTemplate, 
            string classForGeneration, string pathToDirectoryWithFeatures)
        {
            Console.WriteLine("Start generate file");
            var engine = new RazorLightEngineBuilder()
                .UseFilesystemProject(AppDomain.CurrentDomain.BaseDirectory + "../../../")
                .UseMemoryCachingProvider()
                .Build();
            
            var result = engine.CompileRenderAsync(classTemplate, model).Result;
            File.WriteAllText(pathToDirectoryWithFeatures + "/" + classForGeneration, result);
            Console.WriteLine("File generated");
        }

        private static FeatureModel LoadModel(string modelFileName)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(modelFileName);
            XmlElement model = xDoc.DocumentElement;
            
            FeatureModel featureModel = new FeatureModel();
            
            foreach(XmlNode abstractNode in model)
            {
                if(abstractNode.Attributes.Count > 0)
                {
                    XmlNode attr = abstractNode.Attributes.GetNamedItem("name");
                    Console.WriteLine("---" + attr?.Value);
                }
                
                foreach(XmlNode concreteNodes in abstractNode.ChildNodes)
                {
                    foreach (XmlNode concreteNode in concreteNodes.ChildNodes)
                    {
                        String concreteNodeName = concreteNode.Attributes.GetNamedItem("name").Value;
                        Console.WriteLine("------" + concreteNodeName);

                        foreach (XmlNode concreteNodeItem in concreteNode.ChildNodes)
                        {
                            if (concreteNodeItem.Name.Equals("selected"))
                            {
                                featureModel.addFeatureWithSelectFlag(concreteNodeName, Boolean.Parse(concreteNodeItem.InnerText));
                            }
                        }
                    }
                }
            }
            return featureModel;
        }

        private static void CopyAppDirectoryToEnvironment(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(
                targetDirectory + "/" + sourceDirectory.Split("/").Last());
 
            CopyAll(diSource, diTarget);
            
        }
 
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
 
            foreach (FileInfo fileInfo in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fileInfo.Name);
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }
 
            foreach (DirectoryInfo dirInfoSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(dirInfoSourceSubDir.Name);
                CopyAll(dirInfoSourceSubDir, nextTargetSubDir);
            }
        }
        
    }
}