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

        private ConcurrentDictionary<int, T> Datas;

        public TestDatabase(ModelDescriptor<T> model, DatabaseConfiguration c) : base(model, c)
        {
            Config = c;
            Descriptor = model;

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
            Datas = new ConcurrentDictionary<int, T>();
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
                Datas[data.ID] = data;
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
            T res;
            if (Datas.TryGetValue(id, out res)) return res;
            return null;
        }

        public override List<T> GetAll()
        {
            return new List<T>(Datas.Values);
        }

        public override T GetLast()
        {
            return new List<T>(Datas.Values)[Datas.Values.Count - 1];
        }

        public override bool Insert(T data)
        {
            return Datas.TryAdd(data.ID, data);
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
}
