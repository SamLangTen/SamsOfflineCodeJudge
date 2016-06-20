using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security;
using System.Threading;
namespace SamsOfflineCodeJudge
{
    public class Judger
    {
        private TaskFactory judgingTasks;
        private CancellationTokenSource cts;
        public Judger()
        {
            Results = new List<JudgeResult>();
        }
        /// <summary>
        /// A set of code programmed by a user
        /// </summary>
        public JudgeUnit Unit { get; set; }
        /// <summary>
        /// A set of data defined by judger
        /// </summary>
        public SampleData Data { get; set; }
        /// <summary>
        /// Storage results
        /// </summary>
        public List<JudgeResult> Results { get; set; }
        /// <summary>
        /// Called when compilation finished
        /// </summary>
        public Action OnCompiled;
        /// <summary>
        /// Called when all judging task finished
        /// </summary>
        public Action OnJudgedAll;
        /// <summary>
        /// Called when one judging task finished
        /// </summary>
        public Action OnJudged;
        /// <summary>
        /// Cancel all tasks
        /// </summary>
        public void StopJudging()
        {
            cts.Cancel();
        }
        /// <summary>
        /// Compile source code
        /// </summary>
        public string CompileCode(Compiler Compiler)
        {
            //apply for temp directory and compile code
            //apply for temp directory
            if (!Directory.Exists(Path.GetTempPath() + @"SOCJTemp"))
                Directory.CreateDirectory(Path.GetTempPath() + @"SOCJTemp");
            var codeFilename = Path.GetTempPath() + @"SOCJTemp\" + Path.GetRandomFileName() + "." + JudgerManager.LanguageExtensions[Unit.Language];
            var programFilename = Path.GetTempPath() + @"SOCJTemp\" + Path.GetRandomFileName() + ".exe";
            File.WriteAllText(codeFilename, Unit.Code);
            if (File.Exists(programFilename))
                File.Delete(programFilename);
            //compile code
            var compilerProcess = new Process();
            compilerProcess.StartInfo.FileName = Compiler.Filename;
            compilerProcess.StartInfo.Arguments = Compiler.Argument.Replace(@"{Filename}", codeFilename).Replace(@"{Output}", programFilename);
            Console.WriteLine(compilerProcess.StartInfo.Arguments);
            compilerProcess.StartInfo.UseShellExecute = false;
            compilerProcess.StartInfo.CreateNoWindow = true;
            compilerProcess.Start();
            compilerProcess.WaitForExit();
            File.Delete(codeFilename);
            return programFilename;
        }
        /// <summary>
        /// Run a test
        /// </summary>
        public void TestProgram(string ProgramFilename, DataPair TestData, bool WaitForExit)
        {
            Results.Remove(Results.FirstOrDefault(r => r.Index == Data.Datas.IndexOf(TestData)));
            //define basic options
            var testProcess = new Process();
            testProcess.StartInfo.FileName = ProgramFilename;
            //testProcess.StartInfo.UserName = JudgerManager.RunningUser == null ? Environment.UserName : JudgerManager.RunningUser.Username;
            //testProcess.StartInfo.Password = JudgerManager.RunningUser == null ? new SecureString() : JudgerManager.RunningUser.Password;
            testProcess.StartInfo.UseShellExecute = false;
            testProcess.StartInfo.CreateNoWindow = true;
            testProcess.StartInfo.RedirectStandardInput = true;
            testProcess.StartInfo.RedirectStandardOutput = true;
            testProcess.StartInfo.RedirectStandardError = true;
            var result = new JudgeResult();
            testProcess.Start();
            var startTime = DateTime.Now;
            //input sample data
            testProcess.StandardInput.WriteLine(TestData.InputData);
            testProcess.StandardInput.Close();
            //calc memory size
            int tickTime = 0;
            while (!testProcess.HasExited)
            {
                result.MaximumRAM = result.MaximumRAM < testProcess.PeakVirtualMemorySize64 ? testProcess.PeakVirtualMemorySize64 : result.MaximumRAM;
                tickTime += 100; //calc per 100 ms
                if ((WaitForExit && (tickTime > JudgerManager.MaximumTime)) || (!WaitForExit && (tickTime > Data.LimitTime)))
                {
                    testProcess.Kill();
                    break;
                }
                Thread.Sleep(100);
            }
            //compare output
            result.Index = Data.Datas.IndexOf(TestData);
            result.TotalTime = (DateTime.Now - startTime).TotalMilliseconds;
            result.ExitCode = testProcess.ExitCode;
            //check exit code
            if (result.ExitCode != 0)
            {
                result.Result = JudgeResultEnum.RuntimeError;
                Results.Add(result);
                return;
            }
            //process exits with code 0 
            //start comparing result
            var processOutput = testProcess.StandardOutput.ReadToEnd();
            result.Result = JudgeResultEnum.WrongAnswer;
            if (processOutput.Trim() == TestData.OutputData.Trim()) result.Result = JudgeResultEnum.Accepted;
            //if (processOutput == d.OutputData) result.Result = JudgeResultEnum.Accepted;
            //limitation
            if (result.MaximumRAM > Data.LimitRAM) result.Result &= JudgeResultEnum.MemoryLimitExceeded;
            if (result.TotalTime > Data.LimitTime) result.Result &= JudgeResultEnum.TimeLimitExceeded;
            Results.Add(result);
        }
        /// <summary>
        /// Start Judging with given compiler
        /// </summary>
        /// <param name="Compiler">Compiler used to compile the code</param>
        public void StartJudging(Compiler Compiler, bool WaitForExit)
        {
            var programFilename = CompileCode(Compiler);
            //compilation finished
            OnCompiled();
            //no compiled file means compile failed
            if (!File.Exists(programFilename))
            {
                Results = new List<JudgeResult>(new JudgeResult[] { new JudgeResult() { Result = JudgeResultEnum.CompileError } });
                return;
            }
            //run if compilation successful
            //initialize taskfactory
            judgingTasks = new TaskFactory();
            cts = new CancellationTokenSource();
            Task[] tasks = new Task[Data.Datas.Count];
            //run each sample data
            Data.Datas.ForEach(d =>
            {
                tasks[Data.Datas.IndexOf(d)] = (judgingTasks.StartNew(() =>
                  {
                      TestProgram(programFilename, d, WaitForExit);
                  }, cts.Token).ContinueWith((r) =>
                  {
                      OnJudged();
                  }));
            });
            //binding event notification
            judgingTasks.ContinueWhenAll(tasks, (t) =>
            {

                File.Delete(programFilename);
                OnJudgedAll();
            });
        }
        /// <summary>
        /// Start Juding with default compiler
        /// </summary>
        public void StartJudging(bool WaitForExit)
        {
            StartJudging(CompilerManager.Compilers.FirstOrDefault(r => r.Languages.Contains(Unit.Language)), WaitForExit);
        }
    }
}
