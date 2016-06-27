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
        public object this[int key]
        {
            get
            {
                string property = this.GetProperties()[key];
                return this[property];
            }
            set
            {
                string property = GetProperties()[key];
                this[property] = value;
            }
        }
         public List<string> GetProperties()
        {
            List<string> properties = base.GetProperties(true).Select(x => x.Key).ToList();
            return properties;
        }
    }

    public class ParentObj : ObjWithProp
    {
        public int SomeOtherProp { get; set; }
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

        [Test]
        public void Given_Obj_When_GetPropsWithInstance_Then_GetPropsWithoutItemProp()
        {
            //arrange

            ObjWithProp obj = new ObjWithProp();
            obj.SomeProp = "value1";
            //act
            List<string> properties = obj.GetProperties(true).Select(x=>x.Key).ToList();
            //assert
            CollectionAssert.DoesNotContain(properties,"Item");

        }

        [Test]
        public void Given_Obj_When_GetPropsWithInstance_Then_GetProps()
        {
            //arrange

            ObjWithProp obj = new ObjWithProp();
            obj.SomeProp = "value1";
            //act
            List<string> properties = obj.GetProperties(true).Select(x => x.Key).ToList();
            //assert
            CollectionAssert.AreEqual(new List<string>() {"SomeProp"}, properties);

        }

        [Test]
        public void Given_ParentObj_When_GetPropsWithInstance_Then_GetBaseAndParentProps()
        {
            //arrange

            ParentObj obj = new ParentObj();
            obj.SomeProp = "value1";
            obj.SomeOtherProp = 1;
            //act
            List<string> properties = obj.GetProperties(true).Select(x => x.Key).ToList();
            //assert
            CollectionAssert.AreEqual(new List<string>() { "SomeOtherProp","SomeProp" }, properties);

        }



    }

}
