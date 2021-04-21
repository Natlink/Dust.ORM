using Dust.ORM.Core;
using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Databases.MySQL;
using Dust.ORM.Core.Models;
using Dust.ORM.Mysql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dust.ORM.Mysql.Database
{

    [Database("mysql")]
    class MysqlDatabase<T> : AbstractDatabase<T> where T : DataModel, new()
    {
        private readonly string ConnectionString;

        public MysqlDatabase(ModelDescriptor<T> model, DatabaseConfiguration c) : base(model, c)
        {
            Config = c;

            ConnectionString = 
                "Server=" + ((MysqlConfiguration) c).IP + 
                ";port=" + ((MysqlConfiguration)c).Port +
                ";Database=" + ((MysqlConfiguration)c).Database +
                ";User Id=" + ((MysqlConfiguration)c).Username + 
                ";password=" + ((MysqlConfiguration)c).Password +
                ";Min Pool Size=0; Max Pool Size = " + ((MysqlConfiguration)c).PoolSize + 
                ";Pooling=" + ((MysqlConfiguration)c).Pooling;
            if(!((MysqlConfiguration)c).Engine.Equals("MyISAM") && !((MysqlConfiguration)c).Engine.Equals("InnoDB"))
            {
                throw new DatabaseException("", "Unmanaged database engine: " + ((MysqlConfiguration)c).Engine);
            }

            using (MySqlConnection co = new MySqlConnection(ConnectionString))
            {
                co.Open();
                if (!co.Ping())
                    throw new DatabaseException(ConnectionString, "Unreacheable database.");
            }

            if (c.ResetbaseOnStartup)
            {
                DropTable();
            }
            CreateTable();
        }


        #region TableSetup
        public bool DropTable()
        {
            // ExecuteUpdate("CREATE TABLE IF NOT EXISTS `" + Descriptor.ModelTypeName + "` (`ID` INT(11))");
            return ExecuteUpdate("DROP TABLE IF EXISTS " + Descriptor.ModelTypeName + "") != 0;
        }

        public override bool ClearTable()
        {
            // Truncate table return 0 if all OK
            return ExecuteUpdate("TRUNCATE TABLE " + Descriptor.ModelTypeName+"") == 0;
        }

        public override bool CreateTable()
        {
            StringBuilder statement = new StringBuilder("CREATE TABLE IF NOT EXISTS `" + Descriptor.ModelTypeName + "` (\n");
            bool first = true;
            string primaryKey = "";
            foreach (PropertyDescriptor p in Descriptor.Props)
            {
                string sqlProp = PrintMySQL(p);
                if (sqlProp.Equals("")) continue;
                statement.Append((first ? "" : ",\n") + sqlProp);
                first = false;
                if (p.PrimaryKey) primaryKey = ", PRIMARY KEY(`"+p.Name+"`)";
            }
            statement.Append(primaryKey+" ) \nENGINE = "+ ((MysqlConfiguration)Config).Engine + "  DEFAULT CHARSET = "+ ((MysqlConfiguration)Config).Charset+" COMMENT = 'Automaticaly created by ORM' AUTO_INCREMENT = 0;");


            return ExecuteUpdate(statement.ToString()) != 0;
        }

        public override bool DeleteTable()
        { 
            return ExecuteUpdate("DROP TABLE IF EXISTS " + Descriptor.ModelTypeName+ " ") != 0;
        }
        #endregion TableSetup

        #region DataUsage
        public override bool Delete(int id)
        {
            return ExecuteInsert("DELETE FROM `"+ Descriptor.ModelTypeName + "` WHERE `ID` = "+id) != 0;
        }

        public override bool Edit(T data)
        {
            string statement = "UPDATE `" + Descriptor.ModelTypeName + "` SET ";
            bool first = true;
            foreach (PropertyDescriptor p in Descriptor.Props)
            {
                if (p.PropertyType.Equals(typeof(DateTime)))
                {
                    statement += (first ? "" : ", ") + "`" + p.Name + "` = '" + ((DateTime)Descriptor.GetValue(data, p.Name)).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                else if (p.PropertyType.IsEnum)
                {
                    statement += (first ? "" : ", ") + "`" + p.Name + "` = '" + ((int)Descriptor.GetValue(data, p.Name)) + "'";
                }
                else
                {
                    statement += (first ? "" : ", ") + "`" + p.Name + "` = '" + Descriptor.GetValue(data, p.Name) + "'";
                }

                first = false;
            }
            statement += " WHERE `ID` = "+data.ID;
            return ExecuteInsert(statement) != 0;
        }

        public override bool Exist(int id)
        {
            return Read(ExecuteReader("SELECT * FROM `" + Descriptor.ModelTypeName + "` WHERE `ID` = " + id)) != null;
        }

        public override T Get(int id)
        {
            return Read(ExecuteReader("SELECT * FROM `"+Descriptor.ModelTypeName+"` WHERE `ID` = "+id));
        }

        public override List<T> GetAll(int row = -1)
        {
            List<T> res = new List<T>();
            T a = null;
            var reader = ExecuteReader("SELECT * FROM `" + Descriptor.ModelTypeName + (row == -1 ? "`" : "` LIMIT " + row + "," + Config.GetAllSize));
            do
            {
                a = Read(reader);
                if (a != null) res.Add(a);
            } while (a != null);
            return res;
        }


        public override List<T> Get(RequestDescriptor request, int row = -1)
        {
            List<T> res = new List<T>();
            T a = null;
            string condition = "WHERE `"+request.PropertyName+"`";
            switch (request.Op)
            {
                case RequestOperator.Equal: condition += " = "; break;
                case RequestOperator.NotEqual: condition += " != "; break;
                case RequestOperator.Greater: condition += " > "; break;
                case RequestOperator.GreaterOrEqual: condition += " >= "; break;
                case RequestOperator.Less: condition += " < "; break;
                case RequestOperator.LessOrEqual: condition += " <= "; break;
                case RequestOperator.And: condition += " AND "; break;
                case RequestOperator.Or: condition += " OR "; break;
            }
            condition += "'"+request.Value+"' ";
            var reader = ExecuteReader("SELECT * FROM `" + Descriptor.ModelTypeName + "` " + condition + (row == -1 ? "" : "LIMIT " + row + "," + Config.GetAllSize));
            do
            {
                a = Read(reader);
                if (a != null) res.Add(a);
            } while (a != null);
            return res;
        }


        public override T GetLast()
        {
            return Read(ExecuteReader("SELECT * FROM "+Descriptor.ModelTypeName+" WHERE ID=(SELECT MAX(ID) FROM "+Descriptor.ModelTypeName+");"));
        }

        public override bool Insert(T data)
        {
            string statement = "INSERT INTO `" + Descriptor.ModelTypeName + "` ( "; 
            bool first = true;
            foreach(PropertyDescriptor p in Descriptor.Props)
            {
                if (!p.ActiveProperty || (p.Name.Equals("ID") && ((int)Descriptor.GetValue(data, p.Name)) == -1)) continue;
                statement += (first ? "" : ", ") + "`" + p.Name + "`";
                first = false;
            }
            statement += ") VALUES (";
            first = true;
            foreach (PropertyDescriptor p in Descriptor.Props)
            {
                if (!p.ActiveProperty || (p.Name.Equals("ID") && ((int)Descriptor.GetValue(data, p.Name)) == -1)) continue;
                if (p.PropertyType.Equals(typeof(DateTime)))
                {
                    statement += (first ? "" : ",") + " '" + ((DateTime)Descriptor.GetValue(data, p.Name)).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                else if (p.PropertyType.IsEnum)
                {
                    statement += (first ? "" : ",") + " '" + (int)Descriptor.GetValue(data, p.Name) + "'";
                }
                else
                {
                    statement += (first ? "" : ",") + " '" + Descriptor.GetValue(data, p.Name) + "'";
                }
                first = false;
            }
            statement += ")";
            return ExecuteInsert(statement) != 0;
        }

        public override bool InsertAll(List<T> data, bool ID = false)
        {
            string statement = "INSERT INTO `" + Descriptor.ModelTypeName + "` ( ";
            bool first = true;
            foreach (PropertyDescriptor p in Descriptor.Props)
            {
                if (!p.ActiveProperty) continue;
                statement += (first ? "" : ", ") + "`" + p.Name + "`";
                first = false;
            }
            statement += ") VALUES ";
            first = true;
            foreach(T d in data)
            {
                if (first) first = false;
                else statement += ",";
                bool firstProp = true;
                foreach (PropertyDescriptor p in Descriptor.Props)
                {
                    if (!p.ActiveProperty) continue;
                    if (p.PropertyType.Equals(typeof(DateTime)))
                    {
                        statement += (firstProp ? "(" : ",") + " '" + ((DateTime)Descriptor.GetValue(d, p.Name)).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                    else if (p.PropertyType.IsEnum)
                    {
                        statement += (first ? "" : ",") + " '" + (int)Descriptor.GetValue(data, p.Name) + "'";
                    }
                    else
                    {
                        statement += (firstProp ? "(" : ",") + " '" + Descriptor.GetValue(d, p.Name) + "'";
                    }
                    firstProp = false;
                }
                statement += ")";
            }
            return ExecuteInsert(statement) != 0;
        }


        public override T Read(IDataReader reader)
        {
            if (reader.Read())
            {
                return Descriptor.Construct(reader);
            }
            return null;
        }
        #endregion DataUsage

        #region SQLDriverCall

        public MysqlDataReader ExecuteReader(string querry)
        {
            try
            {
                using (MySqlConnection co = new MySqlConnection(ConnectionString))
                {
                    co.Open();
                    using (MySqlCommand cmd = new MySqlCommand(querry, co))
                    {
                        using (MySqlDataReader r = cmd.ExecuteReader())
                        {
                            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
                            while (r.Read())
                            {
                                Dictionary<string, object> dic = new Dictionary<string, object>();
                                res.Add(dic);
                                for (int i = 0; i < r.FieldCount; ++i)
                                {
                                    dic.Add(r.GetName(i), r.GetValue(i));
                                }
                            }
                            r.Close();
                            co.Close();
                            DebugLog("V", querry);
                            return new MysqlDataReader(res);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                DebugLog("X", querry);
                throw new DatabaseException(querry, e.GetType()+": "+e.Message);
            }
        }

        public int ExecuteUpdate(string querry)
        {
            try
            {
                using (MySqlConnection co = new MySqlConnection(ConnectionString))
                {
                    co.Open();
                    using (MySqlCommand cmd = new MySqlCommand(querry, co))
                    {
                        int res = cmd.ExecuteNonQuery();
                        DebugLog("V", querry);
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog("X", querry);
                throw new DatabaseException(querry, e.GetType() + ": " + e.Message);
            }
        }

        public int ExecuteInsert(string querry, out int LastID)
        {
            try
            {
                using (MySqlConnection co = new MySqlConnection(ConnectionString))
                {
                    co.Open();
                    using (MySqlCommand cmd = new MySqlCommand(querry, co))
                    {
                        int res = cmd.ExecuteNonQuery();
                        LastID = (int)cmd.LastInsertedId;
                        DebugLog("V", querry);
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog("X", querry);
                throw new DatabaseException(querry, e.GetType() + ": " + e.Message);
            }
        }
        public int ExecuteInsert(string querry) { return ExecuteInsert(querry, out _); }

        #endregion SQLDriverCall


        public string PrintMySQL(PropertyDescriptor p)
        {
            string res = "";
            if (p.PropertyAttribute != null)
            {
                res = " `" + p.Name + "` ";
                if (p.PropertyType.IsEnum)
                {
                    res += " INT";
                }
                else
                {
                    switch (p.PropertyType.Name)
                    {
                        case "Int32": res += " INT"; break;
                        case "Boolean": res += " BOOLEAN"; break;
                        case "String": res += " VARCHAR"; break;
                        case "DateTime": res += " DATETIME"; break;
                        default: throw new PropertyException(p, "Unmanaged type by MySQL ORM: "+p.PropertyType);
                    }
                }
                if (p.PropertyAttribute.Size != 0)
                {
                    res += "(" + p.PropertyAttribute.Size + ")";
                }
                if (p.PropertyAttribute.NotNull)
                {
                    res += " NOT NULL ";
                }
                if (p.PropertyAttribute.PrimaryKey)
                {
                    res += " AUTO_INCREMENT ";
                }
                if (p.PropertyAttribute.DefaultValue == null) res += "";
                else if (p.PropertyAttribute.DefaultValue.Equals("NULL")) res += " DEFAULT NULL ";
                else if (p.PropertyAttribute.DefaultValue.Equals("CURRENT_TIMESTAMP")) res += " DEFAULT CURRENT_TIMESTAMP ";
                else res += " DEFAULT '" + p.PropertyAttribute.DefaultValue + "' ";
            }
            else if (p.ForeignKey)
            {
                res = " `" + p.Name + "`  INT (11) DEFAULT NULL";
            }
            else if (p.Parsable)
            {
                res = " `" + p.Name + "` VARCHAR (2048) DEFAULT NULL";
            }
            else if (p.Enumerable)
            {
                res = " `" + p.Name + "` VARCHAR (2048) DEFAULT NULL";
            }
            return res;
        }

        public void DebugLog(string head, string core)
        {
            if (((MysqlConfiguration)Config).DebugLog)
                lock (mutex)
                    File.AppendAllText("MYSQL_DEBUG.log", "[" + head + "] " + core + "\n");

        }

        private static object mutex = new();
    }
}
