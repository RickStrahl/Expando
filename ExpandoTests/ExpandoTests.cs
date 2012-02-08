using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Westwind.Utilities;
using System.Dynamic;
using Westwind.Utilities.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Diagnostics;

namespace ExpandoTests
{


    

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

            // iterate over all properties dynamic and native
            foreach (var prop in ex.GetProperties(true))
            {
                Console.WriteLine(prop.Key + " " + prop.Value);
            }

            // you can access plain properties both as explicit or dynamic
            Assert.AreEqual(ex.Name, exd.Name, "Name doesn't match");

            // You can access dynamic properties either as dynamic or via IDictionary
            Assert.AreEqual(exd.Company, ex["Company"] as string, "Company doesn't match");
            Assert.AreEqual(exd.Address, ex["Address"] as string, "Name doesn't match");

            // You can access strong type properties via the collection as well (inefficient though)
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

            // Dictionary pseudo implementation
            ex["Count"] = 10;
            ex["Type"] = "NEWAPP";

            // Dictionary Count - 2 dynamic props added
            Assert.IsTrue(ex.Properties.Count == 4);

               // iterate over all properties
            foreach (KeyValuePair<string, object> prop in exd.GetProperties(true))
            {
                Console.WriteLine(prop.Key + " " + prop.Value);
            }            
        }

        [TestMethod]
        public void MixInObjectInstanceTest()
        {
            // Create expando an mix-in second objectInstanceTest
            var ex = new ExpandoInstance( new Address() );
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            // create dynamic
            dynamic exd = ex;

            // values should show Addresses initialized values (not null)
            Console.WriteLine(exd.Address);
            Console.WriteLine(exd.Email);
            Console.WriteLine(exd.Phone);
        }


        [TestMethod]
        public void JsonSerializeTest()
        {
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            string address = "32 Kaiea";

            ex["Address"] = address;
            ex["Contacted"] = true;
            
            dynamic exd = ex;
            exd.Count = 10;
            exd.Completed = DateTime.Now.AddHours(2);


            JavaScriptSerializer ser = new JavaScriptSerializer();
            Console.WriteLine("*** Serialized Native object:");
            Console.WriteLine(ser.Serialize(ex));
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("*** Serialized Dynamic object:");
            Console.WriteLine(ser.Serialize(exd));
            Console.WriteLine();
        }

        [TestMethod]
        public void XmlSerializeTest()
        {
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            string address = "32 Kaiea";

            ex["Address"] = address;
            ex["Contacted"] = true;

            dynamic exd = ex;
            exd.Count = 10;
            exd.Completed = DateTime.Now.AddHours(2);

            string xml;
            SerializationUtils.SerializeObject(ex, out xml);

            Console.WriteLine(xml);            
        
            ExpandoInstance ex2 = SerializationUtils.DeSerializeObject(xml, typeof(ExpandoInstance)) as ExpandoInstance;

            Assert.IsNotNull(ex2);
            Assert.IsTrue(ex2["Address"] as string == address);
        }

        [TestMethod]
        public void ExpandoObjectJsonTest()
        {
            dynamic ex = new ExpandoObject();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            string address = "32 Kaiea";

            ex.Address = address;
            ex.Contacted = true;
            
            ex.Count = 10;
            ex.Completed = DateTime.Now.AddHours(2);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            string json = ser.Serialize(ex);
        
            Console.WriteLine(json);                        
        }

        [TestMethod]
        public void ExpandoObjectXmlTest()
        {
            dynamic ex = new ExpandoObject();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            string address = "32 Kaiea";

            ex.Address = address;
            ex.Contacted = true;

            ex.Count = 10;
            ex.Completed = DateTime.Now.AddHours(2);

            string xml;
            Assert.IsTrue(SerializationUtils.SerializeObject(ex as ExpandoObject, out xml, true));
            
            
            Console.WriteLine(xml);
        }

        [TestMethod]
        public void UserExampleTest()
        {            
    var user = new User();

    // Set strongly typed properties
    user.Email = "rick@west-wind.com";
    user.Password = "nonya123";
    user.Name = "Rickochet";
    user.Active = true;

    // Now add dynamic properties
    dynamic duser = user;
    duser.Entered = DateTime.Now;
    duser.Accesses = 1;

    // you can also add dynamic props via indexer 
    user["NickName"] = "AntiSocialX";
    duser["WebSite"] = "http://www.west-wind.com/weblog";
            
    // Access strong type through dynamic ref
    Assert.AreEqual(user.Name,duser.Name);

    // Access strong type through indexer 
    Assert.AreEqual(user.Password,user["Password"]);
            

    // access dyanmically added value through indexer
    Assert.AreEqual(duser.Entered,user["Entered"]);
            
    // access index added value through dynamic
    Assert.AreEqual(user["NickName"],duser.NickName);
            

    // loop through all properties dynamic AND strong type properties (true)
    foreach (var prop in user.GetProperties(true))
    { 
        object val = prop.Value;
        if (val == null)
            val = "null";

        Console.WriteLine(prop.Key + ": " + val.ToString());
    }
        }

        [TestMethod]
        public void ExpandoMixinTest()
        {
            // have Expando work on Addresses
            var user = new User( new Address() );

            // cast to dynamicAccessToPropertyTest
            dynamic duser = user;

            // Set strongly typed properties
            duser.Email = "rick@west-wind.com";
            user.Password = "nonya123";
            
            // Set properties on address object
            duser.Address = "32 Kaiea";
            //duser.Phone = "808-123-2131";

            // set dynamic properties
            duser.NonExistantProperty = "This works too";

            // shows default value Address.Phone value
            Console.WriteLine(duser.Phone);

        }
    }

    [Serializable]
    public class ExpandoInstance : Westwind.Utilities.Dynamic.Expando
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
        { }
    }


    [Serializable]
    public class Address
    {
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Address()
        {
            FullAddress = "32 Kaiea";
            Phone = "808 NO-HATE";
            Email = "rick@whatsa.com";
        }
    }

    public class User : Westwind.Utilities.Dynamic.Expando
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime? ExpiresOn { get; set; }

        public User() : base()
        { }

        // only required if you want to mix in seperate instance
        public User(object instance)
            : base(instance)
        {
        }
    }

}
