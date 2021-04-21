using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Databases
{
    interface IDatabase<T> where T : DataModel, new()
    {
        public ModelDescriptor<T> Descriptor { get; }

        public bool CreateTable();
        public bool ClearTable();
        public bool DeleteTable();
        public List<T> GetAll(int row = -1);
        public T Get(int id);
        public bool Exist(int id);
        public bool Delete(int id);
        public int Insert(T data);
        public bool InsertAll(List<T> data, bool ID = false);
        public T GetLast();
        public bool Edit(T data);
        public T Read(IDataReader reader);

        public List<T> Get(RequestDescriptor request, int row = -1);

    }
}
