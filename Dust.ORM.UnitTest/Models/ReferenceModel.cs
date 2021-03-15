using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dust.ORM.CoreTest.Models
{


    class ReferenceModel : DataModel
    {
        [Property(false, 10, null)]
        public int TestValue { get; set; }

        [ForeignID(typeof(SubReferenceModel))]
        public int LinkValue { get; set; }
        public SubReferenceModel LinkValue_ref { get; set; }

        public ReferenceModel()
        {
        }

        public ReferenceModel(int id, int testValue, int linkValue, SubReferenceModel linkValue_ref) : base(id)
        {
            TestValue = testValue;
            LinkValue = linkValue;
            LinkValue_ref = linkValue_ref;
        }

        public override string ToString()
        {
            return "ReferenceModel{id: " + ID + ", TestValue: " + TestValue + ", LinkValue: "+LinkValue+", LinkValue_ref: "+LinkValue_ref?.ToString()+"}";
        }
    }

    class SubReferenceModel : DataModel
    {
        [Property(false, 10, null)]
        public int SubValue { get; set; }

        public SubReferenceModel(int id, int subValue) : base(id)
        {
            SubValue = subValue;
        }

        public SubReferenceModel()
        {
        }

        public override string ToString()
        {
            return "SubReferenceModel{id: "+ID+", SubValue: "+SubValue+"}";
        }
    }

}
