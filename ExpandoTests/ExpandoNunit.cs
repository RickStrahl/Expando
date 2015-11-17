using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Westwind.Utilities.Dynamic;

namespace ExpandoTests
{


    public class ObjWithProp : Expando
    {
        public string SomeProp { get; set; }
    }

    [TestFixture]
    class ExpandoNunit
    {
            [Test]
            public void Given_Porp_When_SetWithIndex_Then_PropsValue()
            {
                //arrange
                ObjWithProp obj = new ObjWithProp();
                //act
                obj.SomeProp = "value1";
                obj["SomeProp"] = "value2";
                //assert
                Assert.AreEqual("value2", obj.SomeProp);

            }
        }
    
}
