using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Westwind.Utilities;
using System.Dynamic;
using Westwind.Utilities.Dynamic;

namespace FlyoutMenuTests
{

    public class ExpandoInstance : Expando
    {
        public string Name { get; set; }
        public DateTime Entered { get; set; }


        public ExpandoInstance() { }

        /// <summary>
        /// Allow passing in of an instance
        /// </summary>
        /// <param name="instance"></param>
        public ExpandoInstance(object instance)
            : base(instance)
        {}        
    }

    /// <summary>
    /// Summary description for ExpandoTests
    /// </summary>
    [TestClass]
    public class ExpandoTests
    {
        public ExpandoTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Summary method that demonstrates some
        /// of the basic behaviors.
        /// 
        /// More specific tests are provided below
        /// </summary>
        [TestMethod]
        public void ExandoModesTest()
        {
            // Set standard properties
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            // set dynamic properties
            dynamic exd = ex;
            exd.Company = "West Wind";
            exd.Accesses = 10;

            
            // set dynamic properties as dictionary
            ex["Address"] = "32 Kaiea";
            ex["Email"] = "rick@west-wind.com";
            ex["TotalOrderAmounts"] = 51233.99M;


            // iterate over all 'dynamic' properties
            foreach (KeyValuePair<string,object> prop in exd)
            {
                Console.WriteLine(prop.Key + " " + prop.Value);
            }

            // you can access plain properties both as explicit or dynamic
            Assert.AreEqual(ex.Name, exd.Name, "Name doesn't match");

            // You can access dynamic properties either as dynamic or via IDictionary
            Assert.AreEqual(exd.Company, ex["Company"] as string, "Company doesn't match");
            Assert.AreEqual(exd.Address, ex["Address"] as string, "Name doesn't match");

            // You can access explicit properties via the collection as well (inefficient though)
            Assert.AreEqual(ex.Name, ex["Name"] as string);

            // dynamic can access everything
            Assert.AreEqual(ex.Name, exd.Name);  // native property
            Assert.AreEqual(ex["TotalOrderAmounts"], exd.TotalOrderAmounts); // dictionary property
        }


        [TestMethod]
        public void AddAndReadDynamicPropertiesTest()
        {
            // strong typing first
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            // create dynamic and create new props
            dynamic exd = ex;
            string company = "West Wind";
            int count = 10;

            exd.Company = company;
            exd.Accesses = count;

            Assert.AreEqual(exd.Company, company);
            Assert.AreEqual(exd.Accesses, count);
        }

        [TestMethod]
        public void AddAndReadDynamicIndexersTest()
        {
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            string address = "32 Kaiea";

            ex["Address"] = address;
            ex["Contacted"] = true;

            dynamic exd = ex;

            Assert.AreEqual(exd.Address, ex["Address"]);
            Assert.AreEqual(exd.Contacted, true);
        }


        [TestMethod]
        public void PropertyAsIndexerTest()
        {
            // Set standard properties
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            Assert.AreEqual(ex.Name, ex["Name"]);
            Assert.AreEqual(ex.Entered, ex["Entered"]);
        }

        [TestMethod]
        public void DynamicAccessToPropertyTest()
        {
            // Set standard properties
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            // turn into dynamic
            dynamic exd = ex;
            
            // Dynamic can access 
            Assert.AreEqual(ex.Name, exd.Name);
            Assert.AreEqual(ex.Entered, exd.Entered);
            
        }

        [TestMethod]
        public void IterateOverDynamicPropertiesTest()
        {
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            dynamic exd = ex;
            exd.Company = "West Wind";
            exd.Accesses = 10;

            // Dictionary Count - 2 dynamic props added
            Assert.IsTrue(ex.Count == 2);

            Assert.IsTrue(exd.GetEnumerator() != null);

            // iterate over all 'dynamic' properties
            foreach (KeyValuePair<string, object> prop in exd)
            {
                Console.WriteLine(prop.Key + " " + prop.Value);
            }            
        }

        [TestMethod]
        public void MixInObjectInstanceTest()
        {
            // Create expando an mix-in second objectInstanceTest
            var ex = new ExpandoInstance( new Addresses() );
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            // create dynamic
            dynamic exd = ex;

            // values should show Addresses initialized values (not null)
            Console.WriteLine(exd.Address);
            Console.WriteLine(exd.Email);
            Console.WriteLine(exd.Phone);
        }

    }


    public class Addresses
    {
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Addresses()
        {
            Address = "32 Kaiea";
            Phone = "808 NO-HATE";
            Email = "rick@whatsa.com";
        }
    }

}
