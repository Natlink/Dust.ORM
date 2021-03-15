using System;

namespace Dust.ORM.Core.Models
{

    [ModelClass]
    public abstract class DataModel
    {
        [Property(true, 11)]
        public int ID { get; set; }

        protected DataModel(int iD)
        {
            ID = iD;
        }

        protected DataModel()
        {
            ID = -1;
        }

    }



}
