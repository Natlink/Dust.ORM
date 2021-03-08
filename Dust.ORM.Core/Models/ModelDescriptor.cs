using Dust.ORM.Core.Databases;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dust.ORM.Core.Models
{

    public class ModelDescriptor<T> where T : DataModel, new()
    {
        private readonly Dictionary<string, PropertyDescriptor> _Props;
        public List<Attribute> Attributes { get; private set; }
        public string ModelTypeName { get; private set; }

        public IEnumerable<PropertyDescriptor> Props { get { return _Props.Values; } }

        public ModelDescriptor()
        {
            ModelTypeName = typeof(T).Name;
            Attributes = new List<Attribute>();
            _Props = new Dictionary<string, PropertyDescriptor>();

            Type type = typeof(T);
            foreach(object a in type.GetCustomAttributes(true))
            {
                if(a is Attribute)
                {
                    Attributes.Add(a as Attribute);
                }
            }
            foreach( PropertyInfo p in type.GetProperties())
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
            StringBuilder res = new StringBuilder( "ModelDescriptor<");
            res.Append(typeof(T).Name);
            res.Append(">{\n");
            foreach(PropertyDescriptor p in _Props.Values)
            {
                res.Append('\t');
                res.Append( p.ToString() );
                res.AppendLine();
            }
            res.Append('}');
            return res.ToString();
        }

        public T Construct(IDataReader reader)
        {
            T res = new T();
            foreach(PropertyDescriptor p in Props)
            {
                p.Set(res, reader.GetRaw(p.Name));
            }
            return res;
        }

    }
}
