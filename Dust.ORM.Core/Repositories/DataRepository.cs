using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Repositories
{
    public abstract class DataRepository
    {

        private Type InnerType;

        protected DataRepository(Type t)
        {
            InnerType = t;
        }

        internal DataRepository<T> Cast<T>() where T : DataModel, new()
        {
            try
            {
                return this as DataRepository<T>;
            }catch(Exception e)
            {
                throw new RepositoryException(this, typeof(T), e.Message);
            }
        }

        public abstract bool Exist(long id);
        public abstract object Get(long id);
        
    }

    public class DataRepository<T> : DataRepository, IDataRepository<T> where T : DataModel, new()
    {
        internal IDatabase<T> Database;
        internal IORMManager Manager;

        internal DataRepository(IDatabase<T> database, IORMManager manager) : base(typeof(T))
        {
            Database = database;
            Manager = manager;
        }

        public bool Delete(long id)
        {
            return Database.Delete(id);
        }

        public bool Edit(T data)
        {
            return Database.Edit(data);
        }

        public override bool Exist(long id)
        {
            return Database.Exist(id);
        }

        public override T Get(long id)
        {
            T res = Database.Get(id);

            if (Database.Descriptor.AutoResolveReference && res != null) Manager.ResolveReference<T>(ref res);
            return res; 
        }

        public List<T> GetAll(int row = -1)
        {
            List<T> res = Database.GetAll(row);
            res.RemoveAll((r) => r == null);
            if (Database.Descriptor.AutoResolveReference) Manager.ResolveReference<T>(ref res);
            return res;
        }

        public T GetLast()
        {
            return Database.GetLast();
        }

        public bool Insert(T data, out long id)
        {
            id = 0;
            try
            {
                if (data != null) id = Database.Insert(data);
                return id != 0;
            }catch(Exception)
            {
                return false;
            }
        }

        public bool Insert(T data)
        {
            try
            {
                return data != null && Database.Insert(data) != 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Clear()
        {
            return Database.ClearTable();
        }

        public bool InsertAll(List<T> data, bool ID = false)
        { 
            return data.Count != 0 && Database.InsertAll(data, ID);
        }


        public List<T> Get(RequestDescriptor request, int row = -1)
        {
            if (request != null)
            {
                ///TODO: Request checking whith property type, operator, etc..
                ///
                List<T> res = Database.Get(request, row);
                if (Database.Descriptor.AutoResolveReference) Manager.ResolveReference<T>(ref res);
                return res;
            }
            return new List<T>();
        }
    }
}
