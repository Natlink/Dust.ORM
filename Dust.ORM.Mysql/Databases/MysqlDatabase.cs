using Dust.ORM.Core;
using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Databases.MySQL;
using Dust.ORM.Core.Models;
using Dust.ORM.Mysql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

            Descriptor = model;
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
                ClearTable();
            }
            CreateTable();
        }


        #region TableSetup
        public override bool ClearTable()
        {
            return ExecuteUpdate("TRUNCATE TABLE "+Descriptor.ModelTypeName) != 0;
        }

        public override bool CreateTable()
        {
            StringBuilder statement = new StringBuilder("CREATE TABLE IF NOT EXISTS `" + Descriptor.ModelTypeName + "` (\n");
            bool first = true;
            string primaryKey = "";
            foreach (PropertyDescriptor p in Descriptor.Props)
            {
                statement.Append((first ? "" : ",") + p.PrintMySQL());
                first = false;
                if (p.PrimaryKey) primaryKey = ", PRIMARY KEY(`"+p.Name+"`)";
            }
            statement.Append(primaryKey+" ) ENGINE = "+ ((MysqlConfiguration)Config).Engine + "  DEFAULT CHARSET = "+ ((MysqlConfiguration)Config).Charset+" COMMENT = 'Automaticaly created by ORM' AUTO_INCREMENT = 0;");


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

        public override List<T> GetAll()
        {
            List<T> res = new List<T>();
            T a = null;
            var reader = ExecuteReader("SELECT * FROM `" + Descriptor.ModelTypeName + "`");
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
            string statement = "INSERT INTO `" + Descriptor.ModelTypeName + "` ( "; // VALUES ('DD', '4')";
            bool first = true;
            foreach(PropertyDescriptor p in Descriptor.Props)
            {
                if (p.Name.Equals("ID")) continue;
                statement += (first ? "" : ", ") + "`" + p.Name + "`";
                first = false;
            }
            statement += ") VALUES (";
            first = true;
            foreach (PropertyDescriptor p in Descriptor.Props)
            {
                if (p.Name.Equals("ID")) continue;
                if(p.PropertyType.Equals(typeof(DateTime)))
                {
                    statement += (first ? "" : ",") + " '" + ((DateTime)Descriptor.GetValue(data, p.Name)).ToString("yyyy-MM-dd HH:mm:ss") + "'";
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
                            return new MysqlDataReader(res);
                        }
                    }
                }
            }
            catch(Exception e)
            {
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
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
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
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                throw new DatabaseException(querry, e.GetType() + ": " + e.Message);
            }
        }
        public int ExecuteInsert(string querry) { return ExecuteInsert(querry, out _); }

        #endregion SQLDriverCall

    }
}
