using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using Dust.ORM.Core.Repositories;
using Dust.Utils.Core.Config;
using Dust.Utils.Core.Logs;
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
        public ILogger Logs;

        public ORMConfiguration Config;
        
        private Dictionary<Type, DataRepository> Repos;
        private Dictionary<string, Type> DatabaseTypes;

        public ORMManager(ILogger logs, ORMConfiguration config)
        {
            Logs = logs;

            Repos = new Dictionary<Type, DataRepository>();
            DatabaseTypes = LoadDatabaseType();
            Config = config;
        }

        public ORMManager(ILogger logs, string configurationFilename = "OrmConfiguration.xml")
        {
            if(configurationFilename.IndexOfAny(new char[] { '*', '&', '#', '\\', '/', '\n', '\t'}) != -1)
            {
                throw new ConfigurationException(null, "ORMConfiguration file's name can't contains theses chars: * & # \\ / newLine tabulation.\nSubmited ormConfiguration file name: "+configurationFilename);
            }

            Logs = logs;

            Repos = new Dictionary<Type, DataRepository>();
            DatabaseTypes = LoadDatabaseType();

            Config = ConfigLoader.Load<ORMConfiguration>(configurationFilename, Logs);

        }

        public DataRepository<T> Get<T>() where T : DataModel, new()
        {
            if (Repos.ContainsKey(typeof(T)))
            {
                return Repos[typeof(T)].Cast<T>();
            }
            var repos = new DataRepository<T>(CreateDatabase<T>());
            Repos.Add(typeof(T), repos);
            return repos;
        }
        public DataRepository GetGeneric(Type t)
        {
            MethodInfo GetMethod = GetType().GetMethod("Get");
            var method = GetMethod.MakeGenericMethod(t);
            return (DataRepository)method.Invoke(this, null);
        }

        private IDatabase<T> CreateDatabase<T>() where T: DataModel, new()
        {
            if (DatabaseTypes.ContainsKey(Config.SelectedDatabase))
            {
                DatabaseConfiguration c = Config.Configs.Find(p => p.Name.Equals(Config.SelectedDatabase));
                if (c == null) throw new ConfigurationException(Config, "No configuration founded for database type: " + Config.SelectedDatabase);

                return (IDatabase<T>)Activator.CreateInstance(DatabaseTypes[Config.SelectedDatabase].MakeGenericType(typeof(T)), new ModelDescriptor<T>(), c);
            }
            throw new ConfigurationException(Config, "Not registered database type: " + Config.SelectedDatabase);
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

        public void ResolveReference<T>(ref T model) where T : DataModel, new()
        {
            if (model == null) throw new NullReferenceException("Model can't be null for resolving references.");
            ModelDescriptor<T> descriptor = Repos[typeof(T)].Cast<T>().Database.Descriptor;
            foreach (var p in descriptor.Props)
            {
                if (p.ForeignKey)
                {
                    int id = (int)p.Get(model);
                    DataRepository repo = GetGeneric(p.ForeignType);
                    object refValue = repo.Get(id);
                    descriptor.SetValue(model, p.Name + "_ref", refValue);
                }
            }
        }
    }

}
