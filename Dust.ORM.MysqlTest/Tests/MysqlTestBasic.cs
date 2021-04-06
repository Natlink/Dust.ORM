using Dust.ORM.Core;
using Dust.ORM.Core.Repositories;
using Dust.ORM.CoreTest.Core;
using Dust.ORM.CoreTest.Models;
using Dust.Utils.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Tests
{
    [Collection("Mysql")]
    public class MysqlTestBasic : OrmCoreTestBasic
    {

        public MysqlTestBasic(ITestOutputHelper output) : base(output)
        {
        }

        public override void SetupOrm()
        {
            Manager = new ORMManager(Log, "TestConfigurationBasic.xml");
        }

        public void TestBenchmark()
        {
            Directory.CreateDirectory("benchmarks");
            int[] array = new int[100];
            for (int i = 0; i < 100; ++i)
            {
                array[i] = i;
            }

            try
            {
                var res = Benchmark<TestClass<int>>(10, array);
                File.AppendAllText("benchmarks/TestClass.csv", BenchmarkToCsv<TestClass<int>>(res));
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }


    }
}
