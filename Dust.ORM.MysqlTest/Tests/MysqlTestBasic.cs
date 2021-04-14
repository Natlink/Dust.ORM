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
            Manager = new ORMManager(Log, "OrmExtension", "TestConfigurationBasic.xml");
        }

      //  [Fact]
        public void TestBenchmark()
        {
            Directory.CreateDirectory("benchmarks");
            int[] array = new int[] {
                1, 5, 10, 25, 50, 75, 100, 250, 500, 750, 1000, 
                1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000,
                2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000,
                3200, 3400, 3600, 3800, 4000, 4200, 4400, 4600, 4800, 5000
            };
            try
            {
                var res = Benchmark<TestClass<int>>(40, array);
                File.AppendAllText("benchmarks/TestClass.csv", BenchmarkToCsv<TestClass<int>>(res));
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }


    }
}
