using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class Graph
{
    public string Name;
    public Blackboard Blackboard { get; private set; }
    Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    public bool LoadByFileName(string fileName)
    {
        this.Name = Path.GetFileName(fileName);
        TextAsset ta = Resources.Load<TextAsset>(fileName);
        return Load(ta.text);
    }

    public bool Load(string content)
    {
        Blackboard = new Blackboard();
        JObject jobject = JObject.Parse(content);

        // 加载blackboard
        if (jobject["Graph"]["Blackboard"] != null)
        {
            Node node = new Node();
            Blackboard.Load(jobject["Graph"]["Blackboard"]);
            Blackboard.OnRegiserPort(node);
            nodes.Add(0, node);
        }

        foreach (var jnode in jobject["Graph"]["Nodes"])
        {
            Node node = (Node)Activator.CreateInstance(Type.GetType((string)jnode["Type"]));
            node.Load(this, jnode);
            nodes.Add(node.ID, node);
        }

        if (jobject["Graph"]["ConnectFlow"] != null)
        {
            foreach (var connect in jobject["Graph"]["ConnectFlow"])
            {
                Connection connection = new Connection(this);
                connection.Connect(ConnectType.Flow, (int)connect["Source"], (string)connect["SourcePort"],
                    (int)connect["Target"], (string)connect["TargetPort"]);
            }
        }

        if (jobject["Graph"]["ConnectValue"] != null)
        {
            foreach (var connect in jobject["Graph"]["ConnectValue"])
            {
                Connection connection = new Connection(this);
                connection.Connect(ConnectType.Value, (int)connect["Source"], (string)connect["SourcePort"],
                    (int)connect["Target"], (string)connect["TargetPort"]);
            }
        }

        return true;
    }

    public string Save()
    {

        return "";
    }

    public Node GetNode(int id)
    {
        if (nodes.ContainsKey(id))
            return nodes[id];

        Debug.LogErrorFormat("cant find node by id:{0}", id);
        return null;
    }
}