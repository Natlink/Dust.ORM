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

        public bool PrimaryKey { get => _attribute != null && _attribute.PrimaryKey; }

        public PropertyDescriptor(PropertyInfo p)
        {
            _descriptor = p;
            
            foreach(Attribute a in _descriptor.GetCustomAttributes())
            {
                if( a is PropertyAttribute)
                    _attribute = a as PropertyAttribute;
            }
        }

        public object Get(object data)
        {
            return _descriptor.GetValue(data);
        }

        public void Set(object data, object value)
        {
            _descriptor.SetValue(data, value);
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
