# Expando Class

Class that allows to create extensible, dynamic types that can add properties at runtime. Similar to ExpandoObject() but with the ability to subclass your objects and effectively extend the class instance. You can also mix-in a separate object via constructor parameter.

You can find out more detail from this blog post:
[Creating a dynamic, extensible C# Expando Object](http://www.west-wind.com/weblog/posts/2012/Feb/08/Creating-a-dynamic-extensible-C-Expando-Object)

##Example Usage

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


## License
copyright, West Wind Technologies, 2012

