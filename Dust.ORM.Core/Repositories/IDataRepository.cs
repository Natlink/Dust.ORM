using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Repositories
{
    interface IDataRepository<T> where T : DataModel
    {
        public List<T> GetAll(int row);
        public T Get(long id);
        public bool Exist(long id);
        public bool Delete(long id);
        public bool Insert(T data);
        public bool Insert(T data, out long id);
        public bool InsertAll(List<T> data, bool ID = false);
        public T GetLast();
        public bool Edit(T data);
        public bool Clear();
        public List<T> Get(RequestDescriptor request, int row = -1);
    }
}
