using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dust.ORM.Core.Models
{

    [AttributeUsage(AttributeTargets.Class)]
    public class ModelClassAttribute : Attribute
    {

        public static IEnumerable<Type> GetAllModels()
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(ModelClassAttribute), true).Length > 0 && type != typeof(DataModel))
                    {
                        yield return type;
                    }
                }
            }
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ModelIDAttribute : Attribute
    {
    }

    public class ForeignIDAttribute : Attribute
    {
        public Type ForeignType;
        public ForeignIDAttribute(Type foreign)
        {
            ForeignType = foreign;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreDatabase : Attribute
    {
        public object DefaultValue;

        public IgnoreDatabase(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public bool PrimaryKey;
        public bool NotNull;
        public int Size;
        public string DefaultValue;

        internal PropertyAttribute(bool primaryKey, int size = 0)
        {
            NotNull = true;
            Size = size;
            DefaultValue = null;
            PrimaryKey = primaryKey;
        }

        public PropertyAttribute(bool notNull = true, int size = 0, string defaultValue = null)
        {
            if (defaultValue == null && notNull) throw new DatabaseException("", "Default value can't be null if property can't be null too.");
            NotNull = notNull;
            Size = size;
            DefaultValue = defaultValue;
            PrimaryKey = false;
        }


    }


}
