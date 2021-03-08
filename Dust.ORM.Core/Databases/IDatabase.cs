using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Databases
{
    interface IDatabase<T> where T : DataModel
    {

        public bool CreateTable();
        public bool ClearTable();
        public bool DeleteTable();
        public List<T> GetAll();
        public T Get(int id);
        public bool Exist(int id);
        public bool Delete(int id);
        public bool Insert(T data);
        public T GetLast();
        public bool Edit(T data);
        public T Read(IDataReader reader);
    }
}
