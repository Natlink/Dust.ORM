using Dust.ORM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Dust.ORM.CoreTest.Models
{
    internal static class DataModelElementsTestCases
    {
        public static readonly List<DataModel> ModelTypeTestCase = new List<DataModel>  {
            new TestClass<bool>(),
            new TestClass<int>(),
            new TestClass<string>(),
            new TestClass<DateTime>(),
        };
    }

    internal class TestClass<T> : DataModel
    {
        [Property(false, 10, null)] public T TestValue1 { get; set; }
        [Property(false, 10, null)] public T TestValue2 { get; set;}

        public TestClass(T testValue1, T testValue2)
        {
            TestValue1 = testValue1;
            TestValue2 = testValue2;
        }

        public TestClass() { TestValue1 = default; TestValue2 = default; }
    }

    internal class ModelDescriptorElements
    {
        public static IEnumerable<object[]> ModelDescriptorTestCase
        {
            get
            {
                List<object[]> tmp = new List<object[]>();
                for (int i = 0; i < DataModelElementsTestCases.ModelTypeTestCase.Count; i++)
                    tmp.Add(new[] { DataModelElementsTestCases.ModelTypeTestCase[i] });
                return tmp;
            }
        }
    }
}
