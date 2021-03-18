using Dust.ORM.Core.Databases;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dust.ORM.Core.Models
{

    public class ModelDescriptor
    {
        private readonly Dictionary<string, PropertyDescriptor> _Props;
        public string ModelTypeName { get; protected set; }
        public Type ModelType { get; protected set; }


        public List<Attribute> Attributes { get; protected set; }
        public IEnumerable<PropertyDescriptor> Props { get { return _Props.Values; } }


        public ModelDescriptor(Type modelType)
        {
            ModelType = modelType;
            ModelTypeName = modelType.Name.Replace('`', '_');
            Attributes = new List<Attribute>();
            _Props = new Dictionary<string, PropertyDescriptor>();

            foreach (object a in ModelType.GetCustomAttributes(true))
            {
                if (a is Attribute)
                {
                    Attributes.Add(a as Attribute);
                }
            }
            foreach (PropertyInfo p in ModelType.GetProperties())
            {
                _Props.Add(p.Name, new PropertyDescriptor(p));
            }
        }

        public void SetValue(object data, string prop, object value)
        {
            _Props[prop].Set(data, value);
        }

        public object GetValue(object data, string prop)
        {
            return _Props[prop].Get(data);
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder("ModelDescriptor<");
            res.Append(ModelTypeName);
            res.Append(">{\n");
            foreach (PropertyDescriptor p in _Props.Values)
            {
                res.Append('\t');
                res.Append(p.ToString());
                res.AppendLine();
            }
            res.Append('}');
            return res.ToString();
        }

        public virtual DataModel Construct(IDataReader reader)
        {
            DataModel res = (DataModel)Activator.CreateInstance(ModelType);
            foreach (PropertyDescriptor p in Props)
            {
                p.Set(res, reader.GetRaw(p.Name));
            }
            return res;
        }
    }


    public class ModelDescriptor<T> : ModelDescriptor where T : DataModel, new()
    {

        public ModelDescriptor() : base(typeof(T))
        {
        }


        public override T Construct(IDataReader reader)
        {
            T res = new T();
            foreach(PropertyDescriptor p in Props)
            {
                if (!p.ActiveProperty) continue;
                p.Set(res, reader.GetRaw(p.Name));
            }
            return res;
        }

    }
}
