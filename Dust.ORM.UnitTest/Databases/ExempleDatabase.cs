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

    [Database("exemple")]
    class ExempleDatabase<T> : AbstractDatabase<T> where T : DataModel, new()
    {

        private ConcurrentDictionary<long, ConcurrentDictionary<string, object>> Datas;
        private long NextAutoIncrement = 1;

        public ExempleDatabase(ModelDescriptor<T> model, DatabaseConfiguration c) : base(model, c)
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
            Datas = new ConcurrentDictionary<long, ConcurrentDictionary<string, object>>();
            return true;
        }

        public override bool DeleteTable()
        {
            Datas = null;
            return true;
        }
        #endregion TableSetup

        #region DataUsage
        public override bool Delete(long id)
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

        public override bool Exist(long id)
        {
            return Datas.ContainsKey(id);
        }

        public override T Get(long id)
        {
            ConcurrentDictionary<string, object> res;
            if (Datas.TryGetValue(id, out res)) return Read(new TestDataReader(res));
            return null;
        }

        public override List<T> GetAll(int row = -1)
        {
            List<T> res = new List<T>();
            T a = null;
            var reader = new TestDataReader(Datas.Values.ToArray());
            do
            {
                a = Read(reader);
                if (a != null) res.Add(a);
            } while (a != null);
            if(row != -1)
            {
                if (res.Count >= row)
                {
                    res.RemoveRange(0, row);
                }
                if(Config.GetAllSize < res.Count)
                {
                    res.RemoveRange(Config.GetAllSize, res.Count - Config.GetAllSize);
                }
            }
            return res;
        }

        public override T GetLast()
        {
            if (Datas.Values.Count == 0) return null;
            var reader = new TestDataReader(Datas.Values.ToArray()[Datas.Values.Count - 1]);
            return Read(reader);
        }

        public override long Insert(T data)
        {
            ConcurrentDictionary<string, object> obj = new ConcurrentDictionary<string, object>();

            foreach(var p in Descriptor.Props)
            {
                obj[p.Name] = p.Get(data);
            }
            if( (long)obj["ID"] == 0)
            {
                obj["ID"] = NextAutoIncrement;
                NextAutoIncrement++;
            }
            return Datas.TryAdd((long)obj["ID"], obj)?(long)obj["ID"]:0;
        }

        public override bool InsertAll(List<T> data, bool ID = false)
        {
            bool res = true;
            foreach(T d in data)
            {
                ConcurrentDictionary<string, object> obj = new ConcurrentDictionary<string, object>();
                foreach (var p in Descriptor.Props)
                {
                    obj[p.Name] = p.Get(d);
                }
                if ((long)obj["ID"] == 0)
                {
                    obj["ID"] = NextAutoIncrement;
                    NextAutoIncrement++;
                }
                res = res && Datas.TryAdd((long)obj["ID"], obj);
            }
            return res;
        }

        public override T Read(IDataReader reader)
        {
            if (reader.Read())
            {
                return Descriptor.Construct(reader);
            }
            return null;
        }

        public override List<T> Get(RequestDescriptor request, int row = -1)
        {
            List<T> res = new List<T>();
            T a = null;
            var reader = new TestDataReader(Datas.Values.ToArray());

            PropertyDescriptor descriptor = null;
            try
            {
                foreach (var p in Descriptor.Props)
                {
                    if (p.Name.Equals(request.PropertyName))
                    {
                        descriptor = p;
                        break;
                    }
                }
                if (descriptor == null) return res;
                do
                {
                    a = Read(reader);
                    if (a != null)
                    {
                        bool match = false;
                        object value = Descriptor.GetValue(a, descriptor.Name);
                        switch (request.Op)
                        {
                            case RequestOperator.Equal: match = value == null ? value == request.Value : value.Equals(request.Value); break;
                            case RequestOperator.NotEqual: match = value == null ? value != request.Value : !value.Equals(request.Value); break;
                            case RequestOperator.Greater: match = (double)value > (double)request.Value; break;
                            case RequestOperator.GreaterOrEqual: match = (double)value >= (double)request.Value; break;
                            case RequestOperator.Less: match = (double)value < (double)request.Value; break;
                            case RequestOperator.LessOrEqual: match = (double)value <= (double)request.Value; break;
                            case RequestOperator.And: match = (bool)value && (bool)request.Value; break;
                            case RequestOperator.Or: match = (bool)value || (bool)request.Value; break;
                        }
                        if (match)
                        {
                            res.Add(a);
                        }
                    }
                } while (a != null);
            }
            catch(Exception e)
            {
                throw new RequestException(request, descriptor, a, "Error in request processing:\n" + e.ToString());
            }
            if (row != -1)
            {
                if (res.Count >= row)
                {
                    res.RemoveRange(0, row);
                }
                if (Config.GetAllSize < res.Count)
                {
                    res.RemoveRange(Config.GetAllSize, res.Count - Config.GetAllSize);
                }
            }
            return res;


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

        public long GetLong(string name)
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
