using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SamsOfflineCodeJudge
{
    public class Judger
    {

        public JudgeUnit Unit { get; set; }
        public SampleData Data { get; set; }
        /// <summary>
        /// Start Judging with given compiler
        /// </summary>
        /// <param name="Compiler">Compiler used to compile the code</param>
        public JudgeResult StartJudging(Compiler Compiler)
        {
            var result = new JudgeResult();
            //apply for temp directory and compile code
            //apply for temp directory
            if (!Directory.Exists(Path.GetTempPath() + @"\SOCJTemp"))
                Directory.CreateDirectory(Path.GetTempPath() + @"\SOCJTemp");
            var codeFilename = Path.GetTempPath() + @"\SOCJTemp\" + Path.GetRandomFileName();
            var programFilename = Path.GetTempPath() + @"\SOCJTemp\" + Path.GetRandomFileName();
            File.WriteAllText(codeFilename, Unit.Code);
            if (File.Exists(programFilename))
                File.Delete(programFilename);
            //compile code
            var compilerProcess = new Process();
            compilerProcess.StartInfo.FileName = Compiler.Filename;
            compilerProcess.StartInfo.Arguments = Compiler.Argument.Replace(@"{Filename}", codeFilename).Replace(@"{Output}", programFilename);
            compilerProcess.Start();
            compilerProcess.WaitForExit();
            //no compiled file means compile failed
            if (!File.Exists(programFilename))
            {
                result.Result = JudgeResultEnum.CompileError;
                return result;
            }
                 
            return null;
        }
        /// <summary>
        /// Start Juding with default compiler
        /// </summary>
        public JudgeResult StartJudging()
        {
            return StartJudging(CompilerManager.Compilers.FirstOrDefault(r => r.Languages.Contains(Unit.Language)));
        }
    }
}
