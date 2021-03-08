using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core
{
    public interface IORMLogger
    {

        public void LogLine(string logs);
        public void Log(string logs);
        
    }
}
