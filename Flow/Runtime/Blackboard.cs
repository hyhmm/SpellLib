using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace XFlow
{
    public partial class Blackboard
    {
        Dictionary<string, object> dataSource = new Dictionary<string, object>();
        public Dictionary<string, object> DataSource { get { return dataSource; } private set { dataSource = value; } }

        public void Load(SerBlackboard sb)
        {
            foreach (var value in sb.Values)
            {
                this.AddData(value.Name, value.Value);
            }
        }

        public T GetData<T>(string name)
        {
            if (dataSource.ContainsKey(name))
                return (T)dataSource[name];

            Debug.LogErrorFormat("cant find data by name:{0}", name);
            return default(T);
        }

        public object GetData(string name)
        {
            if (dataSource.ContainsKey(name))
                return dataSource[name];

            Debug.LogErrorFormat("cant find data by name:{0}", name);
            return default(object);
        }

        public void AddData(string name, object data)
        {
            if (dataSource.ContainsKey(name))
                Debug.LogWarningFormat("already exists name:{0}", name);
            dataSource[name] = data;
        }

        public void SetData(string name, object data)
        {
            dataSource[name] = data;
        }

        public object this[string name]
        {
            get { return GetData(name); }
            set { SetData(name, value); }
        }
    }
}
