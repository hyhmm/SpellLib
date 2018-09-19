using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Text;

public class SerNode
{
    public int X;
    public int Y;

    public string Type;
    public int ID;
    public List<string> ValueOut;
    public List<string> ValueIn;
    public List<string> FlowOut;
    public List<string> FlowIn;

    public void Init(Node node)
    {
        ID = node.ID;
        Type = node.GetType().ToString();
        X = node.X;
        Y = node.Y;
        if (node.PortValueInDict.Count > 0)
        {
            ValueOut = new List<string>();
            ValueOut.AddRange(node.PortValueInDict.Keys);
        }

        if (node.PortValueOutDict.Count > 0)
        {
            ValueIn = new List<string>();
            ValueIn.AddRange(node.PortValueOutDict.Keys);
        }

        if (node.FlowInDict.Count > 0)
        {
            FlowIn = new List<string>();
            FlowIn.AddRange(node.FlowInDict.Keys);
        }

        if (node.FlowOutDict.Count > 0)
        {
            FlowOut = new List<string>();
            FlowOut.AddRange(node.FlowOutDict.Keys);
        }
    }   
}

public class SerValue
{
    public string Name;
    public object Value;
}

public class SerBlackboard
{
    public List<SerValue> Values = new List<SerValue>();

    public void Init(Blackboard blackboard)
    {
        if (blackboard.ShowDataList.Count > 0)
        {
            foreach (var iter in blackboard.ShowDataList)
            {
                SerValue sv = new SerValue();
                sv.Name = iter.Name;
                sv.Value = iter.Value;
                Values.Add(sv);
            }
        }
    }
}

public class SerConnect
{
    public ConnectType Type;
    public int Source;
    public string SourcePort;
    public int Target;
    public string TargetPort;

    public void Init(Connection connection)
    {
        Type = connection.connectType;
        Source = connection.sourceNode.ID;
        SourcePort = connection.sourcePort.name;
        Target = connection.targetNode.ID;
        TargetPort = connection.targetPort.name;
    }
}

public struct SerPort
{
    public int NodeId;
    public string Port;
}

public class SerGraph
{
    public List<SerNode> Nodes = new List<SerNode>();
    public SerBlackboard Blackboard;
    public List<SerConnect> Connections = new List<SerConnect>();

    public void Init(Graph graph)
    {
        Blackboard = new SerBlackboard();
        Blackboard.Init(graph.Blackboard);

        foreach (var node in graph.Nodes.Values)
        {
            SerNode sn = new SerNode();
            sn.Init(node);
            Nodes.Add(sn);
        }

        foreach (var connect in graph.Connections)
        {
            SerConnect sc = new SerConnect();
            sc.Init(connect);
            Connections.Add(sc);
        }
    }
}

public partial class Graph
{
    static void MakeDir(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static void CreatePersisitentFile(string fileName, string data)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        MakeDir(Path.GetDirectoryName(path));
        Debug.Log(path);
        TextWriter writer = new StreamWriter(path, false, Encoding.UTF8);
        writer.Write(data);
        writer.Close();

    }

    public string Serialize()
    {
        SerGraph sg = new SerGraph();
        sg.Init(this);
        return JsonConvert.SerializeObject(sg, Formatting.Indented);
    }
}
