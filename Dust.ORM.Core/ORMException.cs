using Dust.ORM.Core.Databases;
using Dust.ORM.Core.Models;
using Dust.ORM.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dust.ORM.Core
{
    public class ORMException : Exception { public ORMException(string message) : base(message) { } }

    public class ConfigurationException : ORMException
    {
        public ORMConfiguration Config;

        public ConfigurationException(ORMConfiguration config, string message = "") : base(message)
        {
            Config = config;
        }

    }

    public class RequestException : ORMException
    {
        public RequestDescriptor Request;
        public PropertyDescriptor Property;
        public DataModel Model;

        public RequestException(RequestDescriptor request, PropertyDescriptor property, DataModel model, string message) : base(message)
        {
            Request = request;
            Property = property;
            Model = model;
        }

        public override string ToString()
        {
            return base.ToString()+"\nRequest: "+(Request == null? "null":Request.ToString())+"\nProperty: " + (Property == null ? "null" : Property.ToString())+ "\nModel: " + (Model == null ? "null" : Model.ToString());
        }
    }

    public class PropertyException : ORMException
    {

        public PropertyDescriptor Property;

        public PropertyException(PropertyDescriptor p, string message = "") : base(message)
        {
            Property = p;
        }

    }

    public class ModelException<T> : ORMException where T : DataModel, new()
    {
        public ModelDescriptor<T> Model;
        public Type InnerType;

        public ModelException(ModelDescriptor<T> m, string message = "") : base(message)
        {
            Model = m;
            InnerType = typeof(T);
        }
    }

    public class DatabaseException : ORMException
    {
        public string Statement;

        public DatabaseException(string statement, string message = "") : base(message)
        {
            Statement = statement;
        }

        public override string ToString()
        {
            return base.ToString()+"\n"+Statement;
        }
    }

    public class RepositoryException : ORMException
    {
        public DataRepository BaseRepo;
        public Type CastType;

        public RepositoryException(DataRepository baseRepo, Type type, string message = "") : base(message)
        {
            BaseRepo = baseRepo;
            CastType = type;
        }
    }

    public class DataException : ORMException
    {

        public DataModel Model;
        public DataException(DataModel model, string message = "") : base(message)
        {
            Model = model;
        }
    }
}
