﻿using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using Dust.ORM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Dust.ORM.Core
{
    public class ORMManager
    {
        public IORMLogger Logs;

        public ORMConfiguration Config;
        
        private Dictionary<Type, DataRepository> Repos;
        private Dictionary<string, Type> DatabaseTypes;


        public ORMManager(IORMLogger logs, string configurationFilename = "OrmConfiguration.xml")
        {
            if(configurationFilename.IndexOfAny(new char[] { '*', '&', '#', '\\', '/', '\n', '\t'}) != -1)
            {
                throw new ORMException("ORMConfiguration file's name can't contains theses chars: * & # \\ / newLine tabulation.\nSubmited ormConfiguration file name: "+configurationFilename);
            }

            Logs = logs;

            Repos = new Dictionary<Type, DataRepository>();
            DatabaseTypes = LoadDatabaseType();

            Config = LoadConfig(configurationFilename);
        }

        public DataRepository<T> Get<T>() where T : DataModel, new()
        {
            try
            {
                if (Repos.ContainsKey(typeof(T)))
                {
                    return Repos[typeof(T)].Cast<T>();
                }
                var repos = new DataRepository<T>(CreateDatabase<T>());
                Repos.Add(typeof(T), repos);
                return repos;
            }
            catch(Exception e)
            {
                ErrorHandler(e);
                return null;
            }
        }

        private IDatabase<T> CreateDatabase<T>() where T: DataModel, new()
        {
            if (DatabaseTypes.ContainsKey(Config.SelectedDatabase))
            {
                DatabaseConfiguration c = Config.Configs.Find(p => p.Name.Equals(Config.SelectedDatabase));
                if (c == null) throw new ORMException("No configuration founded for database type: " + Config.SelectedDatabase);

                return (IDatabase<T>)Activator.CreateInstance(DatabaseTypes[Config.SelectedDatabase].MakeGenericType(typeof(T)), new ModelDescriptor<T>(), c);
            }
            throw new ORMException("Not registered database type: " + Config.SelectedDatabase);
        }

        private ORMConfiguration LoadConfig(string configurationFilename)
        {
            var configsType = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                    if (type.GetCustomAttributes(typeof(DatabaseConfigurationAttribute), true).Length > 0 && !type.Equals(typeof(DatabaseConfiguration)))
                        configsType.Add(type);
             
            XmlSerializer serial = new XmlSerializer(typeof(ORMConfiguration), configsType.ToArray());

            if (!File.Exists(configurationFilename))
            {
                return GenerateConfiguration(configurationFilename, serial);
            }
            try
            {
                using(FileStream s = new FileStream(configurationFilename, FileMode.Open))
                {
                    return (ORMConfiguration)serial.Deserialize(s);
                }
            } catch(Exception e)
            {
                Logs.Log("Exception while loading ORMConfiguration.\n" + e.ToString()+"\nConfiguration reseted to default values. Old configuration saved to old_"+configurationFilename);

                try
                {
                    File.Copy(configurationFilename, "old_" + configurationFilename, true);
                    File.Delete(configurationFilename);
                } catch(Exception ee)
                {
                    Logs.Log("Exception while backing-up ORMConfiguration.\n" + ee.ToString() + "\nConfiguration reseted to default values. Old configuration not saved.");
                }
                
                return GenerateConfiguration(configurationFilename, serial);
            }
        }

        private ORMConfiguration GenerateConfiguration(string configurationFilename, XmlSerializer serial)
        {
            using (FileStream s = new FileStream(configurationFilename, FileMode.Create))
            {
                var res = new ORMConfiguration(Logs);
                serial.Serialize(s, res);
                return res;
            }
        }

        private Dictionary<string, Type> LoadDatabaseType()
        {
            var res = new Dictionary<string, Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var atts = type.GetCustomAttributes<DatabaseAttribute>(true);
                    foreach(var a in atts)
                    {
                        res.Add(a.DatabaseType, type);
                    }
                }
            }
            return res;
        }

        private void ErrorHandler(Exception e)
        {
            if (e is ORMException)
            {
                if (e is DatabaseException)
                {
                    Logs.Log(e.ToString());
                }
                else if (e is RepositoryException)
                {
                    Logs.Log(e.ToString());
                }
                else if(e is PropertyException)
                {
                    Logs.Log(e.ToString());
                }
                else // e is ModelException<InnerType>
                {
                    Logs.Log(e.ToString());
                }
            }
            else
            {
                Logs.Log(e.ToString());
            }
        }
    }

}
