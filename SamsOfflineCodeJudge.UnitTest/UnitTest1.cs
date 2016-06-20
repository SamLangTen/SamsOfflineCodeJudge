using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SamsOfflineCodeJudge.UnitTest
{
    [TestClass]
    public class TestJudger
    {
        [TestMethod]
        public void TestJudgerFunction()
        {
            //read sample data
            var data = new SampleData();
            for (int i = 1; i <= 10; i++)
                data.Datas.Add(new DataPair()
                {
                    InputData = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\testdata\equation\equation" + i.ToString() + ".in"),
                    OutputData = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\testdata\equation\equation" + i.ToString() + ".out"),
                });
            data.LimitRAM = 137438953472;
            data.LimitTime = 1000;
            //load judge unit
            var ju = new JudgeUnit();
            ju.Code = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\ST33\equation.cpp");
            ju.Language = "C++";
            var compiler = new Compiler();
            compiler.Languages = new List<string>();
            compiler.Languages.Add("C");
            compiler.Filename = @"C:\Program Files (x86)\Dev-Cpp\MinGW64\bin\g++.exe";
            compiler.Argument = "-o \"{Output}\" \"{Filename}\"";
            compiler.Name = "GNU C++ Compiler";
            var judger = new Judger();
            judger.Data = data;
            judger.Unit = ju;
            var isFinished = false;
            judger.OnCompiled = (s,e) =>
            {
                Console.WriteLine("Compilation {0}",e.IsCompilationSucceeded);
            };
            judger.OnJudged = (s, e) =>
            {
                Console.WriteLine("Judged one,index:{0}",e.Index);
            };
            judger.OnJudgedAll = () =>
            {
                Console.WriteLine("Index\tRAM\tTime\tExit Code\tResult");
                judger.Results.ForEach((r) => {
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",r.Index,r.MaximumRAM,r.TotalTime,r.ExitCode,r.Result.ToString());
                });
                isFinished = true;
            };
            judger.StartJudging(compiler, true);
            while (!isFinished) ;
            Console.WriteLine();
        }
    }
}
