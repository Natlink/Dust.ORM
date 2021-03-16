using Dust.ORM.Core;
using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dust.ORM.CoreTest.Databases
{

    [Database("test")]
    class TestDatabase<T> : AbstractDatabase<T> where T : DataModel, new()
    {

        private ConcurrentDictionary<int, ConcurrentDictionary<string, object>> Datas;

        public TestDatabase(ModelDescriptor<T> model, DatabaseConfiguration c) : base(model, c)
        {
            Config = c;

            if (c.ResetbaseOnStartup)
            {
                ClearTable();
            }
            CreateTable();
        }


        #region TableSetup
        public override bool ClearTable()
        {
            if(Datas != null)
            {
                Datas.Clear();
                return true;
            }
            return false;
        }

        public override bool CreateTable()
        {
            Datas = new ConcurrentDictionary<int, ConcurrentDictionary<string, object>>();
            return true;
        }

        public override bool DeleteTable()
        {
            Datas = null;
            return true;
        }
        #endregion TableSetup

        #region DataUsage
        public override bool Delete(int id)
        {
            return Datas.TryRemove(id, out _);
        }

        public override bool Edit(T data)
        {
            if (Datas.ContainsKey(data.ID))
            {
                ConcurrentDictionary<string, object> obj = new ConcurrentDictionary<string, object>();

                foreach (var p in Descriptor.Props)
                {
                    obj[p.Name] = p.Get(data);
                }

                Datas[data.ID] = obj;
                return true;
            }
            return false;
        }

        public override bool Exist(int id)
        {
            return Datas.ContainsKey(id);
        }

        public override T Get(int id)
        {
            ConcurrentDictionary<string, object> res;
            if (Datas.TryGetValue(id, out res)) return Read(new TestDataReader(res));
            return null;
        }

        public override List<T> GetAll()
        {
            List<T> res = new List<T>();
            T a = null;
            var reader = new TestDataReader(Datas.Values.ToArray());
            do
            {
                a = Read(reader);
                if (a != null) res.Add(a);
            } while (a != null);
            return res;
        }

        public override T GetLast()
        {
            if (Datas.Values.Count == 0) return null;
            var reader = new TestDataReader(Datas.Values.ToArray()[Datas.Values.Count - 1]);
            return Read(reader);
        }

        public override bool Insert(T data)
        {
            ConcurrentDictionary<string, object> obj = new ConcurrentDictionary<string, object>();

            foreach(var p in Descriptor.Props)
            {
                obj[p.Name] = p.Get(data);
            }

            return Datas.TryAdd(data.ID, obj);
        }

        public override T Read(IDataReader reader)
        {
            if (reader.Read())
            {
                return Descriptor.Construct(reader);
            }
            return null;
        }
        #endregion DataUsage


    }

    class TestDataReader : IDataReader
    {
        ConcurrentDictionary<string, object>[] Datas;
        int Index;

        public TestDataReader(params ConcurrentDictionary<string, object>[] datas)
        {
            Datas = datas;
            Index = -1;
        }

        public void Dispose() {
        }

        public bool GetBool(string name)
        {
            return (bool)Datas[Index][name];
        }

        public DateTime GetDate(string name)
        {
            return (DateTime)Datas[Index][name];
        }

        public int GetInt(string name)
        {
            return (int)Datas[Index][name];
        }


        public string GetString(string name)
        {
            return (string)Datas[Index][name];
        }

        public bool Read()
        {
            Index++;
            return Datas != null && Datas.Length > Index;
        }

        public object GetRaw(string name)
        {
            return Datas[Index][name];
        }

    }

}
