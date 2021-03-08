using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Repositories
{
    public class DataRepository
    {

        private Type InnerType;

        protected DataRepository(Type t)
        {
            InnerType = t;
        }

        internal DataRepository<T> Cast<T>() where T : DataModel
        {
            try
            {
                return this as DataRepository<T>;
            }catch(Exception e)
            {
                throw new RepositoryException(this, typeof(T), e.Message);
            }
        }

    }

    public class DataRepository<T> : DataRepository, IDataRepository<T> where T : DataModel
    {
        internal IDatabase<T> Database;

        internal DataRepository(IDatabase<T> database) : base(typeof(T))
        {
            Database = database;
        }

        public bool Delete(int id)
        {
            return Database.Delete(id);
        }

        public bool Edit(T data)
        {
            return Database.Edit(data);
        }

        public bool Exist(int id)
        {
            return Database.Exist(id);
        }

        public T Get(int id)
        {
            return Database.Get(id);
        }

        public List<T> GetAll()
        {
            try
            {
                return Database.GetAll();
            }
            catch (DatabaseException e)
            {
                Console.WriteLine(e);
                return new List<T>();
            }
        }

        public T GetLast()
        {
            return Database.GetLast();
        }

        public bool Insert(T data)
        {
            try
            {
                return Database.Insert(data);
            }
            catch(DatabaseException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
