using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
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
            judger.OnCompiled = (s, e) =>
            {
                Console.WriteLine("Compilation {0}", e.IsCompilationSucceeded);
            };
            judger.OnJudged = (s, e) =>
            {
                Console.WriteLine("Judged one,index:{0}", e.Index);
            };
            judger.OnJudgedAll = () =>
            {
                Console.WriteLine("Index\tRAM\tTime\tExit Code\tResult");
                judger.Results.ForEach((r) =>
                {
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", r.Index, r.MaximumRAM, r.TotalTime, r.ExitCode, r.Result.ToString());
                });
                isFinished = true;
            };
            judger.StartJudging(compiler, true);
            while (!isFinished) ;
            Console.WriteLine();
        }

        [TestMethod]
        public void TestJudgerClusterFunction()
        {
            //add compiler
            var compiler = new Compiler();
            compiler.Languages = new List<string>();
            compiler.Languages.Add("C");
            compiler.Filename = @"C:\Program Files (x86)\Dev-Cpp\MinGW64\bin\gcc.exe";
            compiler.Argument = "-o \"{Output}\" \"{Filename}\"";
            compiler.Name = "GNU C Compiler";
            CompilerManager.Compilers.Add(compiler);
 
            //add sample data
            var sd_elimination = new SampleData();
            var sd_trees = new SampleData();
            for (int i = 1; i <= 10; i++)
            {
                sd_elimination.Datas.Add(new DataPair()
                {
                    InputData = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\testdata\elimination\elimination" + i.ToString() + ".in"),
                    OutputData = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\testdata\elimination\elimination" + i.ToString() + ".out"),
                });
                sd_trees.Datas.Add(new DataPair()
                {
                    InputData = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\testdata\trees\trees" + i.ToString() + ".in"),
                    OutputData = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\testdata\trees\trees" + i.ToString() + ".out"),
                });
            }
            sd_elimination.LimitRAM = 137438953472;
            sd_trees.LimitRAM = 137438953472;
            sd_elimination.LimitTime = 1000;
            sd_trees.LimitTime = 1000;
            sd_trees.Id = "trees";
            sd_elimination.Id = "elimination";
            //add user
            var ju_DFH = new JudgeUser(); //38
            var ju_WFJ = new JudgeUser(); //22
            ju_DFH.Id = "DFH";
            ju_WFJ.Id = "WFJ";
            var un_trees_DFH = new JudgeUnit();
            un_trees_DFH.Id = "trees";
            un_trees_DFH.Language = "C";
            un_trees_DFH.Code = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\ST38\T02.c");
            var un_elimination_DFH = new JudgeUnit();
            un_elimination_DFH.Id = "elimination";
            un_elimination_DFH.Language = "C";
            un_elimination_DFH.Code = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\ST38\T01.c");
            var un_trees_WFJ = new JudgeUnit();
            un_trees_WFJ.Id = "trees";
            un_trees_WFJ.Language = "C";
            un_trees_WFJ.Code = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\ST22\trees.c");
            var un_elimination_WFJ = new JudgeUnit();
            un_elimination_WFJ.Id = "elimination";
            un_elimination_WFJ.Language = "C";
            un_elimination_WFJ.Code = File.ReadAllText(@"D:\SamsOfflineCodeJudge.TestData01\ST22\elimination.c");
            ju_DFH.Units.Add(un_trees_DFH);
            ju_DFH.Units.Add(un_elimination_DFH);
            ju_WFJ.Units.Add(un_elimination_WFJ);
            ju_WFJ.Units.Add(un_trees_WFJ);
            //add cluster
            var c = new JudgerCluster();
            c.SampleDatas.Add(sd_elimination);
            c.SampleDatas.Add(sd_trees);
            c.JudgeUsers.Add(ju_WFJ);
            c.JudgeUsers.Add(ju_DFH);
            var isFinished = false;
            c.OnCompleteAll = () => {
                isFinished = true;
            };
            c.RunAll();
            while (!isFinished) ;
            Console.WriteLine("{0}'s Report", ju_DFH.Id);
            Console.WriteLine("\t\tElimination:");
            Console.WriteLine("\t\t\t\tIndex\tRAM\tTime\tResult");
            ju_DFH.Reports.Single(r => r.Data.Id == "elimination").Results.ForEach(r => {
                Console.WriteLine("\t\t\t{0}\t{1}\t{2}\t{3}", r.Index, r.MaximumRAM, r.TotalTime, r.Result.ToString());
            });
            Console.WriteLine("\t\tTrees:");
            Console.WriteLine("\t\t\t\tIndex\tRAM\tTime\tResult");
            ju_DFH.Reports.Single(r => r.Data.Id == "trees").Results.ForEach(r => {
                Console.WriteLine("\t\t\t{0}\t{1}\t{2}\t{3}", r.Index, r.MaximumRAM, r.TotalTime, r.Result.ToString());
            });
            Console.WriteLine("{0}'s Report", ju_WFJ.Id);
            Console.WriteLine("\t\tElimination:");
            Console.WriteLine("\t\t\t\tIndex\tRAM\tTime\tResult");
            ju_WFJ.Reports.Single(r => r.Data.Id == "elimination").Results.ForEach(r => {
                Console.WriteLine("\t\t\t{0}\t{1}\t{2}\t{3}", r.Index, r.MaximumRAM, r.TotalTime, r.Result.ToString());
            });
            Console.WriteLine("\t\tTrees:");
            Console.WriteLine("\t\t\t\tIndex\tRAM\tTime\tResult");
            ju_WFJ.Reports.Single(r => r.Data.Id == "trees").Results.ForEach(r => {
                Console.WriteLine("\t\t\t{0}\t{1}\t{2}\t{3}", r.Index, r.MaximumRAM, r.TotalTime, r.Result.ToString());
            });
        }
    }
}
