using Dust.ORM.Core.Models;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Models
{
    public class ModelTest
    {
        private readonly TestLogger Log;
        public ModelTest(ITestOutputHelper output)
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
                Log.Log(descriptor.ToString());
            }catch(Exception e)
            {
                Log.Log(e.ToString());
                Assert.False(true);
            }
            Assert.NotEmpty(descriptor.Props);
            Assert.Equal(model.GetType(), descriptor.ModelType);
        }

    }



}
