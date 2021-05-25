﻿using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Models
{

    public class TestClass<T> : DataModel
    {
        [Property(false, 100, null)] public T TestValue1 { get; set; }
        [Property(false, 100, null)] public T TestValue2 { get; set; }

        public TestClass(long id, T testValue1, T testValue2) : base(id)
        {
            TestValue1 = testValue1;
            TestValue2 = testValue2;
        }

        public TestClass() { ID = 0; TestValue1 = default; TestValue2 = default; }
    }

    public class EnumTestClass : DataModel
    {
        [Property(false, 10, null)] public EnumData Value1 { get; set; }
        [Property(false, 10, null)] public EnumData Value2 { get; set; }

        public EnumTestClass(long id, EnumData value1, EnumData value2) : base(id)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public EnumTestClass() : base()
        {
            ID = 0;
            Value1 = Value2 = default(EnumData);
        }

        public enum EnumData
        {
            DATA_0 = 0,
            DATA_1 = 1,
            DATA_2 = 2,
            DATA_3 = 3,
            DATA_4 = 4,
        }

    }

    public class ReferenceModel : DataModel
    {
        [Property(false, 10, null)]
        public int TestValue { get; set; }

        [ForeignID(typeof(SubReferenceModel))]
        public long LinkValue { get; set; }
        [ForeignRef(typeof(SubReferenceModel))]
        public SubReferenceModel LinkValue_ref { get; set; }

        public ReferenceModel()
        {
        }

        public ReferenceModel(long id, int testValue, int linkValue, SubReferenceModel linkValue_ref) : base(id)
        {
            TestValue = testValue;
            LinkValue = linkValue;
            LinkValue_ref = linkValue_ref;
        }

        public override string ToString()
        {
            return "ReferenceModel{id: " + ID + ", TestValue: " + TestValue + ", LinkValue: " + LinkValue + ", LinkValue_ref: " + LinkValue_ref?.ToString() + "}";
        }
    }

    public class SubReferenceModel : DataModel
    {
        [Property(false, 10, null)]
        public int SubValue { get; set; }

        public SubReferenceModel(long id, int subValue) : base(id)
        {
            SubValue = subValue;
        }

        public SubReferenceModel()
        {
        }

        public override string ToString()
        {
            return "SubReferenceModel{id: " + ID + ", SubValue: " + SubValue + "}";
        }
    }

    public class EnumerableModel : DataModel
    {
        [EnumerableProperty(typeof(int))]
        public List<int> Datas { get; set; }
        [Property(false, 10, null)]
        public int Value { get; set; }

        public EnumerableModel() { }

        public EnumerableModel(long id, List<int> datas, int value) : base(id) {
            Datas = datas;
            Value = value;
        }

        public override string ToString()
        {
            string list = "[";
            foreach(int i in Datas)
            {
                list += i + ",";
            }
            return "EnumerableModel{ID: "+ID+", Value: "+Value+", Datas: "+list+"]}";
        }
    }

    public class ParsableModel : DataModel
    {
        [ParsableProperty]
        public ParsableReference ParsableObject { get; set; }

        [Property(false, 10, null)]
        public int Value { get; set; }

        public ParsableModel() { }

        public ParsableModel(long id, int value, ParsableReference obj ) : base(id)
        {
            ParsableObject = obj;
            Value = value;
        }

        public override string ToString()
        {
            return "ParsableModel{ID: " + ID + ", Value: " + Value + ", ParsableObject: " + ParsableObject.ToString() + "}";
        }
    }

    public class ParsableReference
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }

        public ParsableReference()
        {
        }

        public ParsableReference(int value1, int value2, int value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }

        public override string ToString()
        {
            return Value1 + "#" + Value2 + "#" + Value3;
        }

        public static ParsableReference Parse(string data)
        {
            string[] s = data.Split('#');
            return new ParsableReference(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
        }
    }

}
