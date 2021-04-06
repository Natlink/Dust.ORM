using Dust.ORM.Core;
using Dust.ORM.Core.Models;
using Dust.ORM.Core.Repositories;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Core
{
    public abstract class OrmCoreTest
    {
        protected ORMManager Manager;
        protected readonly TestLogger Log;

        public OrmCoreTest(ITestOutputHelper output)
        {
            Log = new TestLogger(output);
        }

        public abstract void SetupOrm();

        public (TimeSpan modelCreation, TimeSpan unique, int[] quantity, (TimeSpan store, TimeSpan getAll, TimeSpan clearAll)[]) Benchmark<T>(int repeatCount, params int[] quantity) where T : DataModel, new()
        {
            TimeSpan res1, res2;
            Stopwatch watch = new Stopwatch();
            SetupOrm();
            watch.Start();
            DataRepository<T> repo = Manager.Get<T>();
            watch.Stop();
            res1 = watch.Elapsed;

            watch.Restart();
            repo.Insert(new T());
            watch.Stop();
            res2 = watch.Elapsed;
            repo.Clear();

            (TimeSpan store, TimeSpan getAll, TimeSpan clearAll)[] res = new (TimeSpan store, TimeSpan getAll, TimeSpan clearAll)[quantity.Length];
            for(int y = 0; y < quantity.Length; ++y)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < quantity[y]; ++i)
                {
                    list.Add(new T());
                }
                TimeSpan store = new TimeSpan(0), getAll = new TimeSpan(0), clearAll = new TimeSpan(0); 
                for(int repeat = 0; repeat < repeatCount; ++repeat)
                {
                    watch.Restart();
                    repo.InsertAll(list);
                    watch.Stop();
                    store += watch.Elapsed;
                    watch.Restart();
                    List<T> vars = repo.GetAll(0);
                    watch.Stop();
                    getAll += watch.Elapsed;
                    Assert.Equal(quantity[y], vars.Count);
                    watch.Restart();
                    repo.Clear();
                    watch.Stop();
                    clearAll = watch.Elapsed;
                }
                res[y].store = store / repeatCount;
                res[y].getAll = getAll / repeatCount;
                res[y].clearAll = clearAll / repeatCount;
            }

            return (res1, res2, quantity, res);
        }

        public string BenchmarkToString<T>((TimeSpan modelCreation, TimeSpan unique, int[] quantity, (TimeSpan store, TimeSpan getAll, TimeSpan clearAll)[]) benchmarkRes) where T : DataModel, new()
        {
            string res = "Benchmark for model: ";
            ModelDescriptor<T> model = new ModelDescriptor<T>();
            res += model.ToString();
            res += "\nTimes:\n" + "##########################################";
            res += "\n#  Model creation: " + (string.Format("{0:0.0000}", benchmarkRes.modelCreation.TotalSeconds) + "s").PadRight(22)+"#";
            res += "\n#  Unique insert:  " + (string.Format("{0:0.0000}", benchmarkRes.unique.TotalSeconds) + "s").PadRight(22)+"#";
            res += "\n##########################################\n#  Quantity  #    Store    #   GetAll    #  DeletAll   #";
            for(int i =0; i < benchmarkRes.quantity.Length; ++i)
            {
                res += "\n# " + (""+benchmarkRes.quantity[i]).PadLeft(10);
                res += " # " + (string.Format("{0:0.0000}", benchmarkRes.Item4[i].store.TotalSeconds) + "s").PadLeft(11);
                res += " # " + (string.Format("{0:0.0000}", benchmarkRes.Item4[i].getAll.TotalSeconds) + "s").PadLeft(11);
                res += " # " + (string.Format("{0:0.0000}", benchmarkRes.Item4[i].clearAll.TotalSeconds) + "s").PadLeft(11) +" #";
            }
            res += "\n##########################################";
            return res;
        }
        public string BenchmarkToCsv<T>((TimeSpan modelCreation, TimeSpan unique, int[] quantity, (TimeSpan store, TimeSpan getAll, TimeSpan clearAll)[]) benchmarkRes) where T : DataModel, new()
        {
            string res = "";
            for (int i = 0; i < benchmarkRes.quantity.Length; ++i)
            {
                res += ("" + benchmarkRes.quantity[i]).PadLeft(10);
                res += ";" + string.Format("{0:0.0000}", benchmarkRes.Item4[i].store.TotalSeconds).PadLeft(11);
                res += ";" + string.Format("{0:0.0000}", benchmarkRes.Item4[i].getAll.TotalSeconds).PadLeft(11);
                res += ";" + string.Format("{0:0.0000}", benchmarkRes.Item4[i].clearAll.TotalSeconds).PadLeft(11) + "\n";
            }
            return res;
        }
    }
}
