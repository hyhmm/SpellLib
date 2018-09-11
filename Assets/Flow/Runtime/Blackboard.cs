using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class Blackboard {
    Dictionary<string, object> dataSource = new Dictionary<string, object>();

    public void OnRegiserPort(Node node)
    {
        foreach (var data in dataSource)
        {
            node.AddValueOutPort(data.Key, () => { return GetData(data.Key); });
        }
    }
    public void Load(JToken jnode)
    {
        foreach (var jn in jnode)
        {
            string name = (string)jn["Name"];
            string type = (string)jn["Type"];
            string value = (string)jn["Value"];
            if (type == "int")
                this.AddData(name, int.Parse(value));
            else if (type == "float")
                this.AddData(name, float.Parse(value));
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
