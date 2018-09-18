using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

public partial class Node {
    public int ID;
    protected Graph graph;
    protected Blackboard blackboard;
    protected Dictionary<string, ValueOut> portValueOutDict = new Dictionary<string, ValueOut>();
    protected Dictionary<string, ValueIn> portValueInDict = new Dictionary<string, ValueIn>();
    protected Dictionary<string, FlowIn> flowInDict = new Dictionary<string, FlowIn>();
    protected Dictionary<string, FlowOut> flowOutDict = new Dictionary<string, FlowOut>();

    public Dictionary<string, ValueOut> PortValueOutDict { get { return portValueOutDict; } private set { portValueOutDict = value; } }
    public Dictionary<string, ValueIn> PortValueInDict { get { return portValueInDict; } private set { portValueInDict = value; } }
    public Dictionary<string, FlowIn> FlowInDict { get { return flowInDict; } private set { flowInDict = value; } }
    public Dictionary<string, FlowOut> FlowOutDict { get { return flowOutDict; } private set { flowOutDict = value; } }

    public virtual void Load(Graph graph, SerNode sn)
    {
        this.graph = graph;
        this.blackboard = graph.Blackboard;
        this.ID = sn.ID;
        this.X = sn.X;
        this.Y = sn.Y;

        if (sn.ValueIn != null)
        {
            foreach (var jin in sn.ValueIn)
                AddValueInPort(jin);
        }

        if (sn.ValueOut != null)
        {
            foreach (var jout in sn.ValueOut)
                AddValueOutPort(jout);
        }

        if (sn.FlowIn != null)
        {
            foreach (var fi in sn.FlowIn)
                AddFlowIn(fi);
        }

        if (sn.FlowOut != null)
        {
            foreach (var fo in sn.FlowOut)
                AddFlowOut((string)fo);
        }

        this.RegisterPort();
    }

    public virtual void RegisterPort() { }

    public List<Node> GetChildren(List<int> children)
    {
        if (children == null || children.Count == 0)
            return null;
        List<Node> ret = new List<Node>();
        foreach (var child in children)
        {
            ret.Add(graph.GetNode(child));
        }
        return ret;
    }

    public ValueOut AddValueOutPort(string name)
    {
        ValueOut pv = null;
        if (portValueOutDict.TryGetValue(name, out pv))
            return pv;

        pv = new ValueOut(name);
        portValueOutDict.Add(name, pv);
        return pv;
    }

    public ValueOut AddValueOutPort(string name, ValueHandler valueHandler)
    {
        ValueOut pv = null;

        if (!portValueOutDict.TryGetValue(name, out pv))
        {
            pv = new ValueOut(name);
            portValueOutDict.Add(name, pv);
        }

        pv.SetValueHandler(valueHandler);
        return pv;
    }

    public ValueIn AddValueInPort(string name)
    {
        ValueIn pv;
        if (portValueInDict.TryGetValue(name, out pv)){
            return pv;
        }
        pv = new ValueIn(name);
        portValueInDict.Add(name, pv);
        return pv;
    }

    public ValueIn GetValueInPort(string name)
    {
        if (portValueInDict.ContainsKey(name))
            return portValueInDict[name];
        return null;
    }

    public ValueOut GetValueOutPort(string name)
    {
        if (portValueOutDict.ContainsKey(name))
            return portValueOutDict[name];
        return null;
    }

    public FlowIn AddFlowIn(string name, FlowHandler flowHandler)
    {
        FlowIn fi;
        if (!flowInDict.TryGetValue(name, out fi)) {
            fi = new FlowIn(name);
            flowInDict.Add(name, fi);
        }

        fi.SetFlowHandler(flowHandler);
        return fi;
    }

    public FlowIn AddFlowIn(string name)
    {
        FlowIn fi;
        if (flowInDict.TryGetValue(name, out fi))
        {
            return fi;
        }
        fi = new FlowIn(name);
        flowInDict.Add(name, fi);
        return fi;
    }

    public FlowOut AddFlowOut(string name)
    {
        FlowOut fo;
        if (flowOutDict.TryGetValue(name, out fo))
            return fo;

        fo = new FlowOut(name);
        flowOutDict.Add(name, fo);
        return fo;
    }

    public FlowIn GetFlowIn(string name)
    {
        if (flowInDict.ContainsKey(name))
            return flowInDict[name];
        return null;
    }

    public FlowOut GetFlowOut(string name)
    {
        if (flowOutDict.ContainsKey(name))
            return flowOutDict[name];
        return null;
    }
}

