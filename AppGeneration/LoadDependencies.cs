using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AppGeneration
{
    public class LoadDependencies
    {
        List<Link> linkList = new List<Link>();
        public LoadDependencies(string path)
        {
            string[] filePaths = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories);
            string mainPattern = @"\s*@AnnotationList.\w*";
            string patternOnItemClickTO = @"\s*@AnnotationList.OnItemClickTO\w*";
            string patternOnItemClickFROM = @"\s*@AnnotationList.OnItemClickFROM\w*";
            string patternOnLongItemClickTO = @"\s*@AnnotationList.OnLongItemClickTO\w*";
            string patternOnLongItemClickFROM = @"\s*@AnnotationList.OnLongItemClickFROM\w*";
            foreach(var filePath in filePaths)
            {
                using(var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        // check start of the new record lines
                        if (Regex.Match(line, mainPattern).Success)
                        {
                            var splittedPath = filePath.Split("/");
                            var featureName = splittedPath[splittedPath.Length - 1].Split(".java")[0];
                            Link featureLink = null;
                            if (Regex.Match(line, patternOnItemClickTO).Success)
                            {
                                string linkFromFeatureName = line.Split("OnItemClickTO")[1].Split("\"")[1];
                                featureLink = new Link(linkFromFeatureName, featureName, Link.LinkType.SimpleClick);
                            }
                            else if (Regex.Match(line, patternOnItemClickFROM).Success)
                            {
                                string linkToFeatureName = line.Split("OnItemClickFROM")[1].Split("\"")[1];
                                featureLink = new Link(featureName, linkToFeatureName, Link.LinkType.SimpleClick);

                            }
                            else if (Regex.Match(line, patternOnLongItemClickTO).Success)
                            {
                                string linkFromFeatureName = line.Split("OnLongItemClickTO")[1].Split("\"")[1];
                                featureLink = new Link(linkFromFeatureName, featureName, Link.LinkType.LongClick);
                            }
                            else if (Regex.Match(line, patternOnLongItemClickFROM).Success)
                            {
                                
                                string linkToFeatureName = line.Split("OnLongItemClickFROM")[1].Split("\"")[1];
                                featureLink = new Link(featureName, linkToFeatureName, Link.LinkType.LongClick);
                            }

                            if (featureLink != null)
                            {
                                linkList.Add(featureLink);
                            }
                        }
                    }
                }
            }

            int a = 0;
        }
    }
}