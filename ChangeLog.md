# Expando ChangeLog

### Version 1.2

* **Constructor to load Expando from Dictionary<string,object>**  
Added constructor that loads an Expando from a dictionary including child dictionaries.

* **Dynamic Instance Property access to behave closer to static behavior**  
Expando now fails on invalid assignment (wrong type for example) on instance properties and retrieves values off instance before looking at dynamic Dictionary.


### Version 1.1

* **Fix Serialization with JSON.NET**<br/>
Fix Serialization with JSON.NET to support retrieving both static and dynamic properties.

* **Updated Tests**<br/>
Added new tests for XML and JSON.NET serialization. Fix mix-in test and clarify several of the other tests 

