using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dust.ORM.Core;
using Dust.Utils.Core.Config;
using Dust.Utils.Core.Logs;

namespace Dust.ORM.Core
{
    [Serializable]
    public class ORMConfiguration : DustConfig
    {
        public string ExtensionFolder;
        public string SelectedDatabase;
        public List<DatabaseConfiguration> Configs;

        
        public ORMConfiguration()
        {
            Configs = new List<DatabaseConfiguration>();
            SelectedDatabase = Configs.Count > 0 ? Configs[0].Name : "default";
            ExtensionFolder = "OrmExtension";
        }

        public ORMConfiguration(string extensionFolder, string selectedDatabase, List<DatabaseConfiguration> configs)
        {
            ExtensionFolder = extensionFolder;
            SelectedDatabase = selectedDatabase;
            Configs = configs;
        }
        /*
        public List<DatabaseConfiguration> ScanDatabase()
        {
            var configs = new List<DatabaseConfiguration>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {

                }
            }
            return configs;
        }*/

        internal void AddToConfigurations(Type type)
        {
            foreach(var a in Configs)
            {
                if (a.GetType().Equals(type))
                {
                    return;
                }
            }
            Configs.Add((DatabaseConfiguration)Activator.CreateInstance(type));
        }
    }

    [Serializable]
    [DatabaseConfigurationAttribute]
    public abstract class DatabaseConfiguration {
        public abstract string Name { get; set; }
        public bool ResetbaseOnStartup = false;
        public int GetAllSize = 20;

        protected DatabaseConfiguration(string name, bool resetbaseOnStartup, int getAllSize)
        {
            Name = name;
            ResetbaseOnStartup = resetbaseOnStartup;
            GetAllSize = getAllSize;
        }

    }


}
