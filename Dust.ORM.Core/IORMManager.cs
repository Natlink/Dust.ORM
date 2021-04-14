using Dust.ORM.Core.Models;
using Dust.ORM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dust.ORM.Core
{
    public interface IORMManager
    {

        public DataRepository<T> Get<T>() where T : DataModel, new();

        public DataRepository GetGeneric(Type t);

        void ResolveReference<T>(ref T model) where T : DataModel, new();

        void ResolveReference<T>(ref List<T> modelList) where T : DataModel, new();

    }
}
