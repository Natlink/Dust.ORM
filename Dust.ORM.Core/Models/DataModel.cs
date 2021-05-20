using System;

namespace Dust.ORM.Core.Models
{

    [ModelClass]
    public abstract class DataModel
    {
        [Property(true, 16)]
        public long ID { get; set; }

        protected DataModel(long iD)
        {
            ID = iD;
        }

        protected DataModel()
        {
            ID = -1;
        }

    }



}
