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
    public class ForeignIDAttribute : Attribute
    {
        public Type ForeignType { get; private set; }

        public ForeignIDAttribute(Type foreign)
        {
            ForeignType = foreign;
            if (!ForeignType.IsAssignableTo(typeof(DataModel)))
            {
                throw new ORMException("Foreign ID Attribute property can only a DataModel sub-class type.");
            }
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignRefAttribute : Attribute
    {
        public Type ForeignRefType { get; private set; }

        public ForeignRefAttribute(Type foreign)
        {
            ForeignRefType = foreign;
            if (!ForeignRefType.IsAssignableTo(typeof(DataModel)))
            {
                throw new ORMException("Foreign ID Attribute property can only a DataModel sub-class type.");
            }
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
            if (defaultValue == null && notNull) throw new ORMException("Default value can't be null if property can't be null too.");
            NotNull = notNull;
            Size = size;
            DefaultValue = defaultValue;
            PrimaryKey = false;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EnumerablePropertyAttribute : Attribute
    {
        public Type EnumerableType;
        public string EntitySplitter;

        public EnumerablePropertyAttribute(Type type, string splitter = "/#")
        {
            EnumerableType = type;
            EntitySplitter = splitter;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ParsablePropertyAttribute : Attribute
    {
        public string ParseMethodName;

        public ParsablePropertyAttribute(string parseMethodName = "Parse")
        {
            ParseMethodName = parseMethodName;
        }
    }

}
