using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dust.ORM.Core;

namespace Dust.ORM.Core
{
    [Serializable]
    public class ORMConfiguration
    {
        public string SelectedDatabase;
        public List<DatabaseConfiguration> Configs;

        public ORMConfiguration() { }

        public ORMConfiguration(IORMLogger logs = null)
        {
            Configs = new List<DatabaseConfiguration>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {

                    if (type.GetCustomAttributes(typeof(DatabaseConfigurationAttribute), true).Length > 0)
                    {
                        if (logs != null) logs.Log("Loaded configuration type: " + type);
                        if(!type.Equals(typeof(DatabaseConfiguration))) Configs.Add((DatabaseConfiguration)Activator.CreateInstance(type));
                    }
                }
            }
            SelectedDatabase = Configs.Count > 0? Configs[0].Name:"default";
        }
    }

    [Serializable]
    [DatabaseConfigurationAttribute]
    public abstract class DatabaseConfiguration {
        public abstract string Name { get; set; }
        public bool ResetbaseOnStartup = false;
    }

}
