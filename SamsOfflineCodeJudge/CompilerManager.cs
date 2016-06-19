using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
namespace SamsOfflineCodeJudge
{
    /// <summary>
    /// Load compiler list defined by user
    /// </summary>
    public static class CompilerManager
    {
        public static List<Compiler> Compilers { get; set; }
        public static void LoadCompilerList(string JsonFilename)
        {
            var file = new FileStream(JsonFilename, FileMode.Open);
            LoadCompilerList(file);
        }
        public static void LoadCompilerList(Stream JsonStream)
        {
            var jsonText = new StreamReader(JsonStream).ReadToEnd();
            Compilers = JsonConvert.DeserializeObject<List<Compiler>>(jsonText);
        }
        public static string SaveCompilerList()
        {
            return JsonConvert.SerializeObject(Compilers);
        }
        public static void SaveCompilerList(string Filename)
        {
            File.WriteAllText(Filename, SaveCompilerList());
        }
        
    }
}
