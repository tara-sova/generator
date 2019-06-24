using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using RazorLight;

namespace AppGeneration
{
    class Program
    {
        private static string modelFileName = "generatedModel.xml";
        private static string classForGeneration = "LectureListActivity.java";
        private static string classTemplate = "LectureListActivityTemplate.cshtml";
        private static string pathToDirectoryWithFeatures = "/Users/p.tarasova/RiderProjects/AppGeneration/AppGeneration/adminapp";

        private static string pathToTargetDirectory =
            "/Users/p.tarasova/RiderProjects/AppGeneration/AppGeneration/AppForGeneration/AdminApp/app/src/main/java/com";


        static void Main(string[] args)
        {
            AppGenerator.generate(
                modelFileName, classForGeneration, classTemplate, pathToDirectoryWithFeatures, pathToTargetDirectory);
        }

        

    }
}