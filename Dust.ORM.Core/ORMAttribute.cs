using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core
{

    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseAttribute : Attribute
    {
        public string DatabaseType;

        public DatabaseAttribute(string databaseType)
        {
            DatabaseType = databaseType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DatabaseConfigurationAttribute : Attribute
    {
    }
}
