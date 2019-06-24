using System;
using System.IO;

namespace AppGeneration
{
    class Program
    {
        private static string modelFileName = "generatedModel.xml";
        private static string classForGeneration = "LectureListActivity.java";
        private static string classTemplate = "LectureListActivityTemplate.cshtml";
        private static string pathToDirectoryWithFeatures = "adminapp";

        private static string pathToTargetDirectory =
            "AppForGeneration/AdminApp/app/src/main/java/com";


        static void Main(string[] args)
        {
            string currentDirectoryPath = Directory.GetCurrentDirectory();
            AppGenerator.generate(Path.Combine(currentDirectoryPath, modelFileName), 
                classForGeneration,
                classTemplate, 
                Path.Combine(currentDirectoryPath, pathToDirectoryWithFeatures),
                Path.Combine(currentDirectoryPath, pathToTargetDirectory));
        }
    }
}
