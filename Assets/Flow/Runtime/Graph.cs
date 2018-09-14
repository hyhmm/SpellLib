using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public partial class Graph
{
    public string Name;
    public Blackboard Blackboard { get; private set; }
    private Node blackboardNode;
    Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    public Dictionary<int, Node> Nodes { get { return nodes; } private set { nodes = value; } }
    public List<Connection> Connections = new List<Connection>();

    public bool LoadByFileName(string fileName)
    {
        this.Name = Path.GetFileName(fileName);
        TextAsset ta = Resources.Load<TextAsset>(fileName);
        return Load(ta.text);
    }

    public bool Load(string content)
    {
        Blackboard = new Blackboard();
        SerGraph sg = JsonConvert.DeserializeObject<SerGraph>(content);

        // 加载blackboard
        if (sg.Blackboard != null)
        {
            blackboardNode = new Node();
            Blackboard.Load(sg.Blackboard);
            Blackboard.OnRegiserPort(blackboardNode);
        }

        foreach (var sn in sg.Nodes)
        {
            Node node = (Node)Activator.CreateInstance(Type.GetType(sn.Type));
            node.Load(this, sn);
            nodes.Add(node.ID, node);
        }

        foreach (var connect in sg.Connections)
        {
            Connection connection = new Connection(this);
            connection.Connect(connect.Type, connect.Source, connect.SourcePort, connect.Target, connect.TargetPort);
            Connections.Add(connection);
        }        

        return true;
    }

    public Node GetNode(int id)
    {
        if (id == 0)
        {
            return blackboardNode;
        }

        if (nodes.ContainsKey(id))
            return nodes[id];

        Debug.LogErrorFormat("cant find node by id:{0}", id);
        return null;
    }
}