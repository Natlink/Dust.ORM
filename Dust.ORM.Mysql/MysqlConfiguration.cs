using Dust.ORM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dust.ORM.Mysql
{

    [Serializable]
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

        public MysqlConfiguration() : base("mysql", false, 20)
        {
        }

        public MysqlConfiguration(string iP, int port, string database, string username, string password, bool pooling, int poolSize, string engine, string charset, bool debugLog, bool resetBase, int getAllSize) 
            : base("mysql", resetBase, getAllSize)
        {
            IP = iP;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
            Pooling = pooling;
            PoolSize = poolSize;
            Engine = engine;
            Charset = charset;
            DebugLog = debugLog;
        }

        public override string ToString()
        {
            return Database + ", " + Username + ", " + Password;
        }
    }
}
