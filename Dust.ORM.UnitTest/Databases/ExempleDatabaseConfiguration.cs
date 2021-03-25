using Dust.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dust.ORM.CoreTest.Databases
{
    public class ExempleDatabaseConfiguration : DatabaseConfiguration
    {
        public override string Name { get; set; } = "exemple";


        public ExempleDatabaseConfiguration()
            : base("exemple", true)
        {
        }
    }
}
