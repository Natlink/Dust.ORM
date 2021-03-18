using Dust.ORM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dust.ORM.Mysql
{

    public class MysqlConfiguration : DatabaseConfiguration
    {
        public override string Name { get; set; } = "mysql";
        public string IP = "127.0.0.1";
        public int Port = 3306;
        public string Database = "database_name";
        public string Username = "database_user";
        public string Password = "database_pass";
        public bool Pooling = false;
        public int PoolSize = 1;
        public string Engine = "InnoDB";
        public string Charset = "utf8";
        public bool DebugLog = false;

        public override string ToString()
        {
            return Database + ", " + Username + ", " + Password;
        }
    }
}
