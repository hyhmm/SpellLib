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
    List<EventNode> eventNodes = new List<EventNode>();    

    public bool Load(string name)
    {
        this.Name = name;
        Blackboard = new Blackboard();
        TextAsset ta = Resources.Load<TextAsset>(name);
        JObject jobject = JObject.Parse(ta.text);

        if (jobject["Graph"]["Blackboard"] != null)
        {
            Node node = new Node();
            Blackboard.Load(jobject["Graph"]["Blackboard"]);
            nodes.Add(0, node);
            Blackboard.OnRegiserPort(node);
        }

        foreach (var jnode in jobject["Graph"]["Nodes"])
        {
            Node node = (Node)Activator.CreateInstance(Type.GetType((string)jnode["Type"]));
            node.Init(this, jnode);
            nodes.Add(node.ID, node);

            if (node is EventNode)
                eventNodes.Add(node as EventNode);
        }

        foreach (var node in nodes.Values)
        {
            //Debug.Log(node);
            node.RegisterPort();
          
        }
        foreach (var node in nodes.Values)
            node.Load();

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

    public void Started(SpellAgent sa)
    {
        foreach (var node in eventNodes)
        {
            node.Started(sa);
        }
    }

    public void Stoped(SpellAgent sa)
    {
        foreach (var node in eventNodes)
        {
            node.Stoped(sa);
        }
    }
}