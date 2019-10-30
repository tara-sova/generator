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
            GenerateModelXml(pathToDirectoryWithFeatures,  modelFileName);
//            FeatureModel model = LoadModel(modelFileName);
//            int a = 0;
//            GenerateMainFile(model, classTemplate, classForGeneration, pathToDirectoryWithFeatures).Wait();
//            CopyAppDirectoryToEnvironment(pathToDirectoryWithFeatures, pathToTargetDirectory, model);
        }
        
        private static void GenerateModelXml(string path, string generatedModelFileName)
        {
            List<FeatureWithParameters> featureListForModelGeneration = LoadFeaturesWithParams(path);
            XmlModelGenerator.GenerateXmlModel(featureListForModelGeneration, generatedModelFileName);
        }
        private static List<FeatureWithParameters> LoadFeaturesWithParams(string path)
        {
            List<FeatureWithParameters> featureListForModelGeneration = new List<FeatureWithParameters>();

            string[] filePaths = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories);
            string annotationPattern = @"\s*@AnnotationList.\w*";
            string patternFeature = @"\s*@AnnotationList.Feature\w*";
            string patternAbstract = @"\s*@AnnotationList.AbstractFeature\w*";
            string patternRequired = @"\s*@AnnotationList.RequiredFeature\w*";
            string patternOnItemClickTO = @"\s*@AnnotationList.OnItemClickTO\w*";
            string patternOnItemClickFROM = @"\s*@AnnotationList.OnItemClickFROM\w*";
            string patternOnLongItemClickTO = @"\s*@AnnotationList.OnLongItemClickTO\w*";
            string patternOnLongItemClickFROM = @"\s*@AnnotationList.OnLongItemClickFROM\w*";
            string patternOnSwipeRightTO = @"\s*@AnnotationList.OnSwipeRightTO\w*";
            string patternOnButtonClickFromArgTO = @"\s*@AnnotationList.OnButtonClickFromArgTO\w*";
            
            string patternXorGroup = @"\s*@AnnotationList.XorGroup\w*";
            string patternOrGroup = @"\s*@AnnotationList.OrGroup\w*";
            string patternAndGroup = @"\s*@AnnotationList.AndGroup\w*";

            string patternXorAbstractGroup = @"\s*@AnnotationList.XorAbstractGroup\w*";
            string patternOrAbstractGroup = @"\s*@AnnotationList.OrAbstractGroup\w*";
            string patternAndAbstractGroup = @"\s*@AnnotationList.AndAbstractGroup\w*";

            
            foreach(var filePath in filePaths)
            {
                if (filePath.Contains("AnnotationList.java")) 
                    continue;
                
                using(var reader = new StreamReader(filePath))
                {
                    string featureName = null;
                    string abstractFeatureName = null;
                    bool requiredFeature = false;
                    string longClickTO = null;
                    string longClickFROM = null;
                    string clickTO = null;
                    string clickFROM = null;
                    
                    string xorGroup = null;
                    string orGroup = null;
                    string andGroup = null;
                    
                    string xorAbstractGroup = null;
                    string orAbstractGroup = null;
                    string andAbstractGroup = null;

                    string swipeRightTO = null;
                    string onButtonClickFromArgTO = null;

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

                            if (Regex.Match(line, patternRequired).Success)
                                requiredFeature = true;
                            
                            if (Regex.Match(line, patternOnSwipeRightTO).Success)
                                swipeRightTO = line.Split("SwipeRightTO")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternOnButtonClickFromArgTO).Success)
                                onButtonClickFromArgTO = line.Split("OnButtonClickFromArgTO")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternXorGroup).Success)
                                xorGroup = line.Split("XorGroup")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternOrGroup).Success)
                                orGroup = line.Split("OrGroup")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternAndGroup).Success)
                                andGroup = line.Split("AndGroup")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternXorAbstractGroup).Success)
                                xorAbstractGroup = line.Split("XorAbstractGroup")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternOrAbstractGroup).Success)
                                orAbstractGroup = line.Split("OrAbstractGroup")[1].Split("\"")[1];
                            
                            if (Regex.Match(line, patternAndAbstractGroup).Success)
                                andAbstractGroup = line.Split("AndAbstractGroup")[1].Split("\"")[1];
                        }
                    }
                    if (featureName != null)
                        featureListForModelGeneration.Add(
                        new FeatureWithParameters(
                            featureName, abstractFeatureName, requiredFeature, 
                            longClickTO, longClickFROM, clickTO, clickFROM,
                            xorGroup, orGroup, andGroup,
                            xorAbstractGroup, orAbstractGroup, andAbstractGroup,
                            swipeRightTO, onButtonClickFromArgTO));
                }
            }

            return featureListForModelGeneration;
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
                 File.WriteAllText(Path.Combine(pathToDirectoryWithFeatures, classForGeneration), result);
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
//                            if (concreteNodeItem.Name.Equals("required"))
//                            {
//                                featureModel.addFeatureWithSelectFlag(concreteNodeName, Boolean.Parse(concreteNodeItem.InnerText));
//                            }
                             }
                         }
                     }
                 }
                 return featureModel;
             }

             private static void CopyAppDirectoryToEnvironment(string sourceDirectory, string targetDirectory, FeatureModel model)
             {
                 DirectoryInfo dirSource = new DirectoryInfo(sourceDirectory);
                 DirectoryInfo dirTarget = 
                     new DirectoryInfo(Path.Combine(targetDirectory, sourceDirectory.Split("/").Last()));

                 FindFeatureFilesThatShouldNotBeLoad(sourceDirectory, model);
 
                 CopyAll(dirSource, dirTarget, model);
            
             }
 
             private static void CopyAll(DirectoryInfo source, DirectoryInfo target, FeatureModel model)
             {
                 Directory.CreateDirectory(target.FullName);
 
                 foreach (FileInfo fileInfo in source.GetFiles())
                 {
                     if (shouldNotBeLoadedFileList.Contains(fileInfo.Name))
                     {
                         continue;
                     }
                
                     Console.WriteLine(@"Copying {0}\{1}", target.FullName, fileInfo.Name);
                     fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                 }
 
                 foreach (DirectoryInfo dirInfoSourceSubDir in source.GetDirectories())
                 {
                     DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(dirInfoSourceSubDir.Name);
                     CopyAll(dirInfoSourceSubDir, nextTargetSubDir, model);
                 }
             }
        
        private static readonly List<string> shouldNotBeLoadedFileList =  new List<string>();

        private static void FindFeatureFilesThatShouldNotBeLoad(string path, FeatureModel model)
        {
            string[] filePaths = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories);
            string patternFeature = @"\s*@AnnotationList.Feature\w*";
            string patternAnyway = @"\s*@AnnotationList.NeededAnywayFeatureFile\w*";
//            string patternFeatureConnected = @"\s*@AnnotationList.ConnectedToFeature\w*";

            foreach(var filePath in filePaths)
            {
                if (filePath.Contains("AnnotationList.java")) 
                    continue;
                
                using(var reader = new StreamReader(filePath))
                {
                    string featureName = null;
                    string neededAnyway = null;
                    string fileName = filePath.Split("/").Last();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (Regex.Match(line, patternFeature).Success)
                        {
                            featureName = line.Split("Feature")[1].Split("\"")[1];
                            if (!model.Features[featureName])
                            {
                                shouldNotBeLoadedFileList.Add(fileName);
                            }
                        }

                        if (Regex.Match(line, patternAnyway).Success)
                        {
                            shouldNotBeLoadedFileList.Remove(fileName);
                        }
                    }
                }
            }
        }
        
    }
}