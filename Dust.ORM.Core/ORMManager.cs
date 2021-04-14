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
    public class ORMManager : IORMManager
    {
        public ILogger Logs;

        public ORMConfiguration Config;


        private Dictionary<Type, DataRepository> Repos;
        private Dictionary<string, Type> DatabaseTypes;

        public ORMManager(ILogger logs, string extensionFolder, string configurationFilename = "OrmConfiguration.xml")
        {
            Logs = logs;
            if (configurationFilename == null || configurationFilename.Length <= 0 || configurationFilename.IndexOfAny(new char[] { '*', '&', '#', '\\', '/', '\n', '\t'}) != -1)
            {
                throw new ConfigurationException(null, "ORMConfiguration file's name can't be null, empty or contains theses chars: * & # \\ / newLine tabulation.\nSubmited ormConfiguration file name: "+configurationFilename);
            }

            List<Assembly> assemblies = LoadAssemblies(extensionFolder);

            Config = ConfigLoader.Load<ORMConfiguration>(configurationFilename, assemblies, Logs);
            Repos = new Dictionary<Type, DataRepository>();
            DatabaseTypes = LoadDatabaseType(assemblies, configurationFilename);
        }

        public DataRepository<T> Get<T>() where T : DataModel, new()
        {
            if (Repos.ContainsKey(typeof(T)))
            {
                return Repos[typeof(T)].Cast<T>();
            }
            var repos = new DataRepository<T>(CreateDatabase<T>(), this);
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

        private Dictionary<string, Type> LoadDatabaseType(List<Assembly> assemblies, string configurationFilename)
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            bool configurationChanged = false;
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var atts = type.GetCustomAttributes<DatabaseAttribute>(true);
                    foreach (var a in atts)
                    {
                        res.Add(a.DatabaseType, type);
                    }
                    if (type.GetCustomAttributes(typeof(DatabaseConfigurationAttribute), true).Length > 0 && !type.Equals(typeof(DatabaseConfiguration)))
                    {
                        configurationChanged |= Config.AddToConfigurations(type);
                    }
                }
            }
            if (configurationChanged)
            {
                ConfigLoader.Save<ORMConfiguration>(configurationFilename, assemblies, Config, Logs);
            }
            return res;
        }

        private List<Assembly> LoadAssemblies(string extensionFolder)
        {
            var res = new List<Assembly>();

            if (!Directory.Exists("./" + extensionFolder))
            {
                Directory.CreateDirectory("./" + extensionFolder);
                return res;
            }
            foreach (string dllName in Directory.GetFiles("./" + extensionFolder, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFrom("./" + dllName);
                if(assembly != null)
                {
                    res.Add(assembly);
                }

            }
            return res;
        }

        public void ResolveReference<T>(ref T model) where T : DataModel, new()
        {
            if (model == null) throw new NullReferenceException("ORM Trying to resolve references on a null object.");
            ModelDescriptor<T> descriptor = Get<T>().Database.Descriptor;

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

        public void ResolveReference<T>(ref List<T> modelList) where T : DataModel, new()
        {
            if (modelList == null) throw new NullReferenceException("ORM Trying to resolve references on a null object.");
            ModelDescriptor<T> descriptor = Get<T>().Database.Descriptor;
            foreach (var p in descriptor.Props)
            {
                if (p.ForeignKey)
                {
                    DataRepository repo = GetGeneric(p.ForeignType);
                    foreach(T tt in modelList)
                    {
                        int id = (int)p.Get(tt);
                        object refValue = repo.Get(id);
                        descriptor.SetValue(tt, p.Name + "_ref", refValue);
                    }
                }
            }
        }

    }

}
