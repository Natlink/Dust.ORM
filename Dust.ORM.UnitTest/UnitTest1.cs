using Dust.ORM.Core;
using Dust.ORM.Core.Models;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.UnitTest
{
    public class UnitTest1
    {
        private readonly TestLogger Log;

        public UnitTest1(ITestOutputHelper output)
        {
            Log = new TestLogger(output);
        }

        ORMManager Manager;

        [Fact]
        public void Test1()
        {
            Manager = new ORMManager(Log);
            var a = Manager.Get<TestClass1>();
            a.Get(0);
        }
    }


    public class TestClass1 : DataModel
    {

        [Property(false, 12, "DefaultName")]
        public string Name1 { get; set; }
    }
}
