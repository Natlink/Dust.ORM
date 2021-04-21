﻿using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core.Databases
{
    public abstract class AbstractDatabase<T> : IDatabase<T> where T : DataModel, new()
    {
        public ModelDescriptor<T> Descriptor { get; private set; }
        public DatabaseConfiguration Config;

        protected AbstractDatabase(ModelDescriptor<T> descriptor, DatabaseConfiguration config)
        {
            Descriptor = descriptor;
            Config = config;
        }

        public abstract bool ClearTable();

        public abstract bool CreateTable();

        public abstract bool Delete(int id);

        public abstract bool DeleteTable();

        public abstract bool Edit(T data);

        public abstract bool Exist(int id);

        public abstract T Get(int id);

        public abstract List<T> GetAll(int row = -1);

        public abstract T GetLast();

        public abstract int Insert(T data);

        public abstract T Read(IDataReader reader);

        public abstract bool InsertAll(List<T> data, bool ID = false);

        public abstract List<T> Get(RequestDescriptor request, int row = -1);
    }
}
