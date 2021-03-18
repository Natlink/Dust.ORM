using Dust.ORM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Dust.ORM.UnitTest
{
    public class TestLogger : IORMLogger
    {

        ITestOutputHelper Output;

        public TestLogger(ITestOutputHelper output)
        {
            Output = output;
        }

        public void Log(string logs)
        {
            Output.WriteLine(logs);
        }

        public void LogLine(string logs)
        {
            Output.WriteLine(logs);
        }
    }
}
