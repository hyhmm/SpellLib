using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace XFlow
{
    public partial class Blackboard
    {
        Dictionary<string, Variable> dataSource = new Dictionary<string, Variable>();
        public Dictionary<string, Variable> DataSource { get { return dataSource; } private set { dataSource = value; } }

        public void Load(SerBlackboard sb)
        {
            foreach (var value in sb.Values)
            {
                this.AddData(value.Name, value.Value);
            }
        }

        public Variable GetData(string name)
        {
            if (dataSource.ContainsKey(name))
                return dataSource[name];

            Debug.LogErrorFormat("cant find data by name:{0}", name);
            return default(Variable);
        }

        public void AddData(string name, Variable data)
        {
            if (dataSource.ContainsKey(name))
                Debug.LogWarningFormat("already exists name:{0}", name);
            dataSource[name] = data;
        }

        public void SetData(string name, Variable data)
        {
            dataSource[name] = data;
        }

        public Variable this[string name]
        {
            get { return GetData(name); }
            set { SetData(name, value); }
        }
    }
}
