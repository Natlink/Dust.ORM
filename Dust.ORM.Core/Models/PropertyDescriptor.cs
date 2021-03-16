using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dust.ORM.Core.Models
{
    public class PropertyDescriptor
    {

        public string Name { get => _descriptor.Name; }
        public Type PropertyType { get => _descriptor.PropertyType; }

        private PropertyInfo _descriptor;
        private PropertyAttribute _attribute;
        private ForeignIDAttribute _foreignAttribute;
        private EnumerablePropertyAttribute _enumerableAttribute;
        private ParsablePropertyAttribute _parsableAttribute;

        public bool PrimaryKey { get => _attribute != null && _attribute.PrimaryKey; }
        public bool ForeignKey { get => _foreignAttribute != null; }
        public Type ForeignType { get => _foreignAttribute?.ForeignType; }
        public Type EnumerableType { get => _enumerableAttribute?.EnumerableType; }
        public bool Enumerable { get => _enumerableAttribute != null; }
        public bool Parsable { get => _parsableAttribute != null; }

        public PropertyDescriptor(PropertyInfo p)
        {
            _descriptor = p;
            
            foreach(Attribute a in _descriptor.GetCustomAttributes())
            {
                if( a is PropertyAttribute)         _attribute = a as PropertyAttribute;
                if( a is ForeignIDAttribute)   _foreignAttribute = a as ForeignIDAttribute;
                if( a is EnumerablePropertyAttribute)   _enumerableAttribute = a as EnumerablePropertyAttribute;
                if( a is ParsablePropertyAttribute)   _parsableAttribute = a as ParsablePropertyAttribute;
            }
            if (PrimaryKey && ForeignKey) throw new PropertyException(this, "Property can't be a primary key and a foreign key at same time.");
            if (ForeignKey && !PropertyType.IsPrimitive) throw new PropertyException(this, "This property with foreign ID attribute must be a primitive.");
        }

        public object Get(object data)
        {
            if (Enumerable)
            {
                object enumerable = Activator.CreateInstance(PropertyType);
                string res = (string)this.GetType().GetMethod("EnumerableToString").
                                        MakeGenericMethod(_enumerableAttribute.EnumerableType).
                                        Invoke(this, new object[] { _descriptor.GetValue(data), _enumerableAttribute.EntitySplitter});
                return res;
            }
            else if (Parsable)
            {
                return _descriptor.GetValue(data).ToString();
            }
            else
            {
                return _descriptor.GetValue(data);
            }
        }

        public void Set(object data, object value)
        {
            if (Enumerable)
            {
                try
                {
                    object enumerable = Activator.CreateInstance(PropertyType);

                    this.GetType().GetMethod("ParseEnumerable").
                        MakeGenericMethod(_enumerableAttribute.EnumerableType).
                        Invoke(this, new object[] { value, _enumerableAttribute.EntitySplitter, enumerable });

                    _descriptor.SetValue(data, enumerable);
                }
                catch (Exception e)
                {
                    throw new PropertyException(this, "Exception while parsing data:"+ value+" : "+_enumerableAttribute.EntitySplitter+"\n"+e.ToString());
                }
            }
            else if (Parsable)
            {
                _descriptor.SetValue(data, PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { value }));
            }
            else
            {
                _descriptor.SetValue(data, value);
            }
        }

        public string EnumerableToString<U>(List<U> list, string splitter)
        {
            if (list == null || list.Count == 0) return "";
            string res = "";
            bool first = true;
            foreach(U u in list)
            {
                if (!first) res += splitter;
                first = false;
                res += u.ToString();
            }
            return res;
        }

        public void ParseEnumerable<U>(string data, string splitter, ref List<U> list)
        {
            if (data == null || data.Equals("")) return;
            string[] datas = data.Split(splitter);
            foreach(string s in datas)
            {
                U obj = (U)typeof(U).GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { s });
                list.Add(obj);
            }
        }


        public override string ToString()
        {
            string att = "";
            foreach (Attribute a in _descriptor.GetCustomAttributes())
            {
                att += (att.Equals("")?"":", ") +a;
            }
            return Name + ": " + PropertyType.Name+ ( att.Equals("")? "" : " ["+att+"]");
        }

        

        public string PrintMySQL()
        {
            if (_attribute == null) throw new PropertyException(this, "Every property need an attribute of type PropertyAttribute.");

            string res = " `" + Name + "` ";
            switch (PropertyType.Name)
            {
                case "Int32": res += " INT"; break;
                case "Boolean": res += " BOOLEAN"; break;
                case "String": res += " VARCHAR"; break;
                case "DateTime": res += " DATETIME"; break;
                default: throw new PropertyException(this, "Unmanaged type by MySQL ORM.");
            }
            if(_attribute.Size != 0)
            {
                res += "(" + _attribute.Size + ")";
            }
            if (_attribute.NotNull)
            {
                res += " NOT NULL ";
            }
            if (_attribute.PrimaryKey)
            {
                res += " AUTO_INCREMENT ";
            }
            if (_attribute.DefaultValue == null) res += "";
            else if (_attribute.DefaultValue.Equals("NULL")) res += " DEFAULT NULL ";
            else if (_attribute.DefaultValue.Equals("CURRENT_TIMESTAMP")) res += " DEFAULT CURRENT_TIMESTAMP ";
            else res += " DEFAULT '"+ _attribute.DefaultValue+"' ";
            return res;
        }

    }
}
