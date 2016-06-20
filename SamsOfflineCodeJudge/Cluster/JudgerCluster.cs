using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SamsOfflineCodeJudge
{
    public class JudgerCluster
    {
        private List<Judger> judgers = new List<Judger>();

        public Action OnCompleteAny;
        public Action OnCompleteAll;
        private List<Action> CancelTasksDelegate = new List<Action>();
        public string Name { get; set; }
        public List<JudgeUser> JudgeUsers { get; set; } = new List<JudgeUser>();
        public List<SampleData> SampleDatas { get; set; } = new List<SampleData>();
        public int MaxTaskCount { get; set; } = 10;
        private void Initialize(List<SampleData> ParticularData, List<JudgeUser> ParticularUser)
        {
            judgers.Clear();
            CancelTasksDelegate.Clear();
            JudgeUsers.Where(j => ParticularUser.Contains(j)).AsQueryable().ToList().ForEach(u => { u.Reports.Clear(); });
            JudgeUsers.Where(j => ParticularUser.Contains(j)).AsQueryable().ToList().ForEach((u) =>
            {
                u.Units.Where(j => ParticularData.Contains(SampleDatas.SingleOrDefault(s => s.Id == j.Id))).AsQueryable().ToList().ForEach((n) =>
                     {
                         var j = new Judger();
                         j.Data = SampleDatas.Single(s => s.Id == n.Id);
                         j.Unit = n;
                         j.OnJudged = (sender, e) => { OnCompleteAny(); };
                         var report = new UserReport() { Data = j.Data, Results = j.Results };
                         u.Reports.Add(report);
                         judgers.Add(j);
                     });
            });
        }
        private void Run()
        {
            Task.Factory.StartNew(() =>
                        {
                            int mtc = MaxTaskCount;
                            int totalFinished = 0;
                            judgers.ForEach((j) =>
                            {
                                while (mtc == 0) ;//MaxTaskCount 0 means stop and wait task finished
                                mtc--;
                                j.OnJudged = (o, e) =>
                                {
                                    mtc++; //a task finished and add this ticker
                                };
                                j.OnJudgedAll = () =>
                                {
                                    totalFinished++;
                                    if (totalFinished >= judgers.Count) OnCompleteAll(); //if totalFinishedTicker equals judger's count means all judger has run
                                };
                                CancelTasksDelegate.Add(new Action(j.StopJudging)); //add stop method delegate
                                j.StartJudging(true);
                            });
                        });
        }
        public void RunAll()
        {
            Initialize(SampleDatas, JudgeUsers);
            Run();
        }
        public void RunByUser(List<string> UserIds)
        {
            Initialize(SampleDatas, JudgeUsers.Where(u => UserIds.Contains(u.Id)).AsQueryable().ToList());
            Run();
        }
        public void RunByData(List<string> DataIds)
        {
            Initialize(SampleDatas.Where(d => DataIds.Contains(d.Id)).AsQueryable().ToList(), JudgeUsers);
            Run();
        }
        public void RunByUserAndData(List<string> UserIds, List<string> DataIds)
        {
            Initialize(SampleDatas.Where(d => DataIds.Contains(d.Id)).AsQueryable().ToList(), JudgeUsers.Where(u => UserIds.Contains(u.Id)).AsQueryable().ToList());
            Run();
        }
        public void Cancel()
        {
            CancelTasksDelegate.ForEach(a => { a(); });
        }
    }
}
