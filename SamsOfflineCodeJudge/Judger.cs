using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security;
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
        public List<JudgeResult> StartJudging(Compiler Compiler, bool WaitForExit)
        {
            var judgeResultCollection = new List<JudgeResult>();
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
            compilerProcess.StartInfo.UseShellExecute = false;
            compilerProcess.StartInfo.CreateNoWindow = true;
            compilerProcess.Start();
            compilerProcess.WaitForExit();
            //no compiled file means compile failed
            if (!File.Exists(programFilename))
                return new List<JudgeResult>(new JudgeResult[] { new JudgeResult() { Result = JudgeResultEnum.CompileError } });
            //run if compilation successful
            //define basic options
            var testProcess = new Process();
            testProcess.StartInfo.FileName = programFilename;
            testProcess.StartInfo.UserName = JudgerManager.RunningUser == null ? Environment.UserName : JudgerManager.RunningUser.Username;
            testProcess.StartInfo.Password = JudgerManager.RunningUser == null ? new SecureString() : JudgerManager.RunningUser.Password;
            testProcess.StartInfo.UseShellExecute = false;
            testProcess.StartInfo.CreateNoWindow = true;
            testProcess.StartInfo.RedirectStandardInput = true;
            testProcess.StartInfo.RedirectStandardOutput = true;
            testProcess.StartInfo.RedirectStandardError = true;
            //run each sample data
            int dataIndex = -1;
            Data.Datas.ForEach(d =>
            {
                dataIndex++;
                var result = new JudgeResult();
                testProcess.Start();
                var startTime = DateTime.Now;
                //input sample data
                testProcess.StandardInput.Write(Data.Datas[dataIndex].InputData);
                if (WaitForExit)
                    testProcess.WaitForExit();
                else
                    testProcess.WaitForExit(Convert.ToInt32(Data.LimitTime));
                //compare output
                result.TotalTime = (DateTime.Now - startTime).TotalMilliseconds;
                result.MaximumRAM = testProcess.PeakVirtualMemorySize64;
                result.ExitCode = testProcess.ExitCode;
                //check exit code
                if (result.ExitCode != 0)
                {
                    result.Result = JudgeResultEnum.RuntimeError;
                    judgeResultCollection.Add(result);
                    return;
                }
                //process exits with code 0 
                //start comparing result
                var processOutput = testProcess.StandardOutput.ReadToEnd();
                result.Result = JudgeResultEnum.WrongAnswer;
                if (processOutput.Trim() == Data.Datas[dataIndex].OutputData.Trim()) result.Result = JudgeResultEnum.PresentationError;
                if (processOutput == Data.Datas[dataIndex].OutputData) result.Result = JudgeResultEnum.Accepted;
                //limitation
                if (result.MaximumRAM > Data.LimitRAM) result.Result &= JudgeResultEnum.MemoryLimitExceeded;
                if (result.TotalTime > Data.LimitTime) result.Result &= JudgeResultEnum.TimeLimitExceeded;
                judgeResultCollection.Add(result);
            });
            return judgeResultCollection;
        }
        /// <summary>
        /// Start Juding with default compiler
        /// </summary>
        public List<JudgeResult> StartJudging(bool WaitForExit)
        {
            return StartJudging(CompilerManager.Compilers.FirstOrDefault(r => r.Languages.Contains(Unit.Language)), WaitForExit);
        }
    }
}
