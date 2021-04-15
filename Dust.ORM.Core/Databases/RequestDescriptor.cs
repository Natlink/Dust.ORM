using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dust.ORM.Core.Databases
{
    public class RequestDescriptor
    {

        public string PropertyName;
        public RequestOperator Op;
        public object Value;

        public RequestDescriptor(string propertyName, RequestOperator op, object value)
        {
            PropertyName = propertyName;
            Op = op;
            Value = value;
        }
    }

    public enum RequestOperator
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        And,
        Or,
    }
}
