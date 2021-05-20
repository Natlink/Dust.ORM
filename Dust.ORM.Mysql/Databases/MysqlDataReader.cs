using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Databases.MySQL
{
    public class MysqlDataReader : IDataReader
    {

        private List<Dictionary<string, object>> Datas { get; set; }
        int index;

        public MysqlDataReader(List<Dictionary<string, object>> datas)
        {
            Datas = datas;
            index = -1;
        }

        public bool Read()
        {
            index++;
            return index < Datas.Count;
        }

        public int GetInt(string name)
        {
            if (Datas[index][name] is int)
                return (int)Datas[index][name];
            else
                return (int)(uint)Datas[index][name];
        }

        public long GetLong(string name)
        {
            if (Datas[index][name] is long)
                return (long)Datas[index][name];
            else
                return (long)(ulong)Datas[index][name];
        }

        public bool GetBool(string name)
        {
            return (bool)Datas[index][name];
        }


        public string GetString(string name)
        {
            return (string)Datas[index][name];
        }

        public DateTime GetDate(string name)
        {
            return (DateTime)Datas[index][name];
        }

        public object GetRaw(string name)
        {
            return Datas[index][name];
        }

        public void Dispose()
        {
            Datas.Clear();
            Datas = null;
        }
    }
}
