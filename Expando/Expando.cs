using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Reflection;

namespace Westwind.Utilities.Dynamic
{
    /// <summary>
    /// Class that provides extensible properties and methods. This
    /// dynamic object stores 'extra' properties in a dictionary or
    /// checks the actual properties of the instance.
    /// 
    /// This means you can subclass this expando and retrieve either
    /// native properties or properties from values in the dictionary.
    /// 
    /// This type allows you three ways to access its properties:
    /// 
    /// Directly: any explicitly declared properties are accessible
    /// Dynamic: dynamic cast allows access to dictionary and native properties/methods
    /// Dictionary: Any of the extended properties are accessible via IDictionary interface
    /// </summary>
    public class Expando : DynamicObject, IDynamicMetaObjectProvider, IDictionary<string,object>
    {
        /// <summary>
        /// Instance of object passed in
        /// </summary>
        object Instance;

        /// <summary>
        /// Cached type of the instance
        /// </summary>
        Type InstanceType;

        /// <summary>
        /// String Dictionary that contains the extra dynamic values
        /// stored on this object/instance
        /// </summary>
        protected Dictionary<string,object> Properties = new Dictionary<string, object>();

        /// <summary>
        /// This constructor just works off the internal dictionary and any 
        /// public properties of this object.
        /// 
        /// Note you can subclass Expando.
        /// </summary>
        public Expando() 
        {
            Initialize(this);
        }

        /// <summary>
        /// Allows passing in an existing instance variable to 'extend'.        
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to 
        /// check native properties and only check the Dictionary!
        /// </remarks>
        /// <param name="instance"></param>
        public Expando(object instance)
        {
            Initialize(instance);
        }

        protected virtual void Initialize(object instance)
        {
            Instance = instance;
            if (instance != null)
                InstanceType = instance.GetType();           
        }

       /// <summary>
       /// Try to retrieve a member by name first from instance properties
       /// followed by the collection entries.
       /// </summary>
       /// <param name="binder"></param>
       /// <param name="result"></param>
       /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            // first check the Properties collection for member
            if (Properties.Keys.Contains(binder.Name))
            {
                result = Properties[binder.Name];
                return true;
            }


            // Next check for Public properties via Reflection
            if (Instance != null)
            {
                try
                {
                    return GetProperty(Instance, binder.Name, out result);                    
                }
                catch { }
            }

            // failed to retrieve a property
            result = null;
            return false;
        }


        /// <summary>
        /// Property setter implementation tries to retrieve value from instance 
        /// first then into this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            // first check to see if there's a native property to set
            if (Instance != null)
            {
                try
                {
                    bool result = SetProperty(Instance, binder.Name, value);
                    if (result)
                        return true;
                }
                catch { }
            }
            
            // no match - set or add to dictionary
            Properties[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Instance != null)
            {
                try
                {
                    // check instance passed in for methods to invoke
                    if (InvokeMethod(Instance, binder.Name, args, out result))
                        return true;                    
                }
                catch { }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Reflection Helper method to retrieve a property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool GetProperty(object instance, string name, out object result)
        {
            if (instance == null)
                instance = this;

            var miArray = InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    result = ((PropertyInfo)mi).GetValue(instance,null);
                    return true;
                }
            }

            result = null;
            return false;                
        }

        /// <summary>
        /// Reflection helper method to set a property value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool SetProperty(object instance, string name, object value)
        {
            if (instance == null)
                instance = this;

            var miArray = InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)mi).SetValue(Instance, value, null);
                    return true;
                }
            }
            return false;                
        }

        /// <summary>
        /// Reflection helper method to invoke a method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool InvokeMethod(object instance, string name, object[] args, out object result)
        {
            if (instance == null)
                instance = this;

            // Look at the instanceType
            var miArray = InstanceType.GetMember(name,
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.Public | BindingFlags.Instance);

            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0] as MethodInfo;
                result = mi.Invoke(Instance, args);
                return true;
            }

            result = null;
            return false;
        }

        #region IDictionary<string,object> implementation


        /// <summary>
        /// Adds custom properties in the Properties collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            Properties.Add(key, value); 
        }

        /// <summary>
        /// Checks for keys in the internal properties dictionary.
        /// 
        /// Does not check properties
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return Properties.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return Properties.Keys; }
        }

        public bool Remove(string key)
        {
            return Properties.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return Properties.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return Properties.Values; }
        }


        /// <summary>
        
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                try
                {
                    // try to get from properties collection first
                    return Properties[key];
                }
                catch (KeyNotFoundException ex)
                {
                    // try reflection on instanceType
                    object result = null;
                    if (GetProperty(Instance, key, out result))
                        return result;

                    // nope doesn't exist
                    throw;
                }
            }
            set
            {
                if (Properties.ContainsKey(key))
                {
                    Properties[key] = value;
                    return;
                }

                // check instance for existance of type first
                var miArray = InstanceType.GetMember(key, BindingFlags.Public | BindingFlags.GetProperty);
                if (miArray != null && miArray.Length > 0)
                    SetProperty(Instance, key, value);
                else
                    Properties[key] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Properties.Add(item.Key, item.Value);            
        }

        public void Clear()
        {
            Properties.Clear();            
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return Properties.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return Properties.Count; }
        }

        public bool IsReadOnly
        {
            get { return false;}
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return Properties.Remove(item.Key); 
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach(var key in this.Properties.Keys)
                yield return new KeyValuePair<string,object>(key,this.Properties[key]);            
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Properties.GetEnumerator(); 
        }

    #endregion
    }
}
