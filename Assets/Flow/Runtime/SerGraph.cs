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

    [JsonIgnore]
    public float Width;
    [JsonIgnore]
    public float Height;

    public string Type;
    public int ID;
    public List<string> ValueOut;
    public List<string> ValueIn;
    public List<string> FlowOut;
    public List<string> FlowIn;

    public Rect Rect
    {
        get { return new Rect(X, Y, Width, Height); }
        set
        {
            X = (int)value.x;
            Y = (int)value.y;
            Width = value.width;
            Height = value.height;
        }
    }

    public int FlowCount
    {
        get { return System.Math.Max(FlowIn == null ? 0 : FlowIn.Count, FlowOut == null ? 0 : FlowOut.Count); }
    }

    public int PortCount
    {
        get { return System.Math.Max(ValueOut == null ? 0 : ValueOut.Count, ValueIn == null ? 0 : ValueIn.Count); }
    }

    public void Init(Node node)
    {
        ID = node.ID;
        Type = node.GetType().ToString();

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

    public int DefaultWidth { get { return 100; } }
    public int DefaultHeight {
        get
        {
            return (FlowCount + PortCount) * 30;
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
        if (blackboard.DataSource.Count > 0)
        {
            foreach (var iter in blackboard.DataSource)
            {
                SerValue sv = new SerValue();
                sv.Name = iter.Key;
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


    public void Save()
    {
        SerGraph sg = new SerGraph();
        sg.Init(this);
        string data = JsonConvert.SerializeObject(sg);
        CreatePersisitentFile("test.json", data);
    }
}
