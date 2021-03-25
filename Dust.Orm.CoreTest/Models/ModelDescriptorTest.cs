using Dust.ORM.Core.Models;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Models
{
    public class ModelDescriptorTest
    {
        private readonly TestLogger Log;
        public ModelDescriptorTest(ITestOutputHelper output)
        {
            Log = new TestLogger(output);
        }

        [Theory]
        [MemberData(nameof(ModelDescriptorElements.ModelDescriptorTestCase), MemberType = typeof(ModelDescriptorElements))]
        public void ModelCreationTest(DataModel model)
        {
            ModelDescriptor descriptor = null;
            try
            {
                descriptor = new ModelDescriptor(model.GetType());
                Log.Info(descriptor.ToString());
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
                Assert.False(true);
            }
            Assert.NotEmpty(descriptor.Props);
            Assert.Equal(model.GetType(), descriptor.ModelType);
        }

    }


    internal static class DataModelElementsTestCases
    {
        public static readonly List<DataModel> ModelTypeTestCase = new List<DataModel>  {
            new TestClass<bool>(),
            new TestClass<int>(),
            new TestClass<string>(),
            new TestClass<DateTime>(),
        };
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
