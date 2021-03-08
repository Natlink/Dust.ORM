using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Databases
{
    public interface IDataReader : IDisposable
    {
        public bool Read();
        public int GetInt(string name);
        public bool GetBool(string name);
        public string GetString(string name);
        public DateTime GetDate(string name);
        public object GetRaw(string name);
    }
}
