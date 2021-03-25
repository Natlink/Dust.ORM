using Dust.ORM.Core;
using Dust.ORM.CoreTest.Core;
using Dust.Utils.Core.Config;
using System;
using System.Collections.Generic;
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

    }
}
