using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

public delegate object ValueHandler();
public delegate void FlowHandler();
public class Port
{
    protected string name;
}

public class ValueInPort : Port
{
    ValueOutPort connectedPort;

    public ValueInPort(string name)
    {
        this.name = name;
    }

    public void Connect(Graph graph, int nodeId, string connectedPort)
    {
        var node = graph.GetNode(nodeId);
        if (node == null)
            return;
        var outport = node.GetValueOutPort(connectedPort);
        if (outport == null)
            return;

        this.connectedPort = outport;
    }

    public object Value
    {
        get { return connectedPort.Value; }
    }
}

public class ValueOutPort : Port
{
    ValueHandler valueHandler;
    public ValueOutPort(string name)
    {
        this.name = name;
    }

    public ValueOutPort(string name, ValueHandler valueHandler)
    {
        this.name = name;
        this.valueHandler = valueHandler;
    }

    public void SetValueHandler(ValueHandler valueHandler)
    {
        this.valueHandler = valueHandler;
    }

    public object Value
    {
        get { return valueHandler(); }
    }
}

public class FlowIn : Port
{
    FlowHandler flowHandler;
    public FlowIn(string name)
    {
        this.name = name;
    }

    public FlowIn(string name, FlowHandler flowHandler)
    {
        this.name = name;
        this.flowHandler = flowHandler;
    }

    public void SetFlowHandler(FlowHandler flowHandler)
    {
        this.flowHandler = flowHandler;
    }

    public void Call()
    {
        if (flowHandler != null)
            flowHandler.Invoke();
    }
}

public class FlowOut : Port
{
    FlowIn connectedFlowInput;
    public FlowOut(string name)
    {
        this.name = name;
    }

    public void Connect(Graph graph, int nodeId, string connectedPort)
    {
        var node = graph.GetNode(nodeId);
        if (node == null)
            return;
        var flowInput = node.GetFlowInput(connectedPort);
        if (flowInput == null)
            return;

        connectedFlowInput = flowInput;
    }

    public void Call()
    {
        if (connectedFlowInput != null)
            connectedFlowInput.Call();
    }
}


public class Node {
    public int ID;
    protected Graph graph;
    protected JToken jnode;
    protected Blackboard blackboard;
    protected Dictionary<string, ValueOutPort> portValueOutDict = new Dictionary<string, ValueOutPort>();
    protected Dictionary<string, ValueInPort> portValueInDict = new Dictionary<string, ValueInPort>();
    protected Dictionary<string, FlowIn> flowInDict = new Dictionary<string, FlowIn>();
    protected Dictionary<string, FlowOut> flowOutDict = new Dictionary<string, FlowOut>();
    public virtual void Init(Graph graph, JToken jnode)
    {
        this.jnode = jnode;
        this.graph = graph;
        this.blackboard = graph.Blackboard;
        this.ID = (int)jnode["ID"];
    }

    public virtual void Load()
    {
        if (jnode == null)
            return;
        if (jnode["ValueIn"] != null)
        {
            foreach (var jin in jnode["ValueIn"])
            {
                var port = this.GetValueInPort((string)jin["Name"]);
                if (port != null)
                    port.Connect(this.graph, (int)jin["Node"], (string)jin["ConnectPort"]);
            }
        }
        if (jnode["FlowOut"] != null)
        { 
            foreach (var jout in jnode["FlowOut"])
            {
                var port = this.GetFlowOut((string)jout["Name"]);
                if (port != null)
                    port.Connect(this.graph, (int)jout["Node"], (string)jout["ConnectPort"]);
            }
        }
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

    public ValueOutPort AddValueOutPort(string name)
    {
        ValueOutPort pv = null;
        if (portValueOutDict.TryGetValue(name, out pv))
            return pv;

        pv = new ValueOutPort(name);
        portValueOutDict.Add(name, pv);
        return pv;
    }

    public ValueOutPort AddValueOutPort(string name, ValueHandler valueHandler)
    {
        ValueOutPort pv = null;
        if (portValueOutDict.TryGetValue(name, out pv))
        {
            pv.SetValueHandler(valueHandler);
            return pv;
        }

        pv = new ValueOutPort(name, valueHandler);
        portValueOutDict.Add(name, pv);
        return pv;
    }

    public ValueInPort AddValueInPort(string name)
    {
        ValueInPort pv;
        if (portValueInDict.TryGetValue(name, out pv)){
            return pv;
        }
        pv = new ValueInPort(name);
        portValueInDict.Add(name, pv);
        return pv;
    }

    public ValueInPort GetValueInPort(string name)
    {
        if (portValueInDict.ContainsKey(name))
            return portValueInDict[name];
        return null;
    }

    public ValueOutPort GetValueOutPort(string name)
    {
        if (portValueOutDict.ContainsKey(name))
            return portValueOutDict[name];
        return null;
    }

    public FlowIn AddFlowIn(string name, FlowHandler flowHandler)
    {
        FlowIn fi;
        if (flowInDict.TryGetValue(name, out fi)) {
            return fi;
        }

        fi = new FlowIn(name, flowHandler);
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

    public FlowIn GetFlowInput(string name)
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

public class EventNode : Node
{
    public virtual void Started(SpellAgent sa) { }

    public virtual void Stoped(SpellAgent sa) { }
}

public class SpellStart : EventNode
{
    SpellAgent spellAgent;
    public override void RegisterPort()
    {
        var spellAgentPort = AddValueOutPort("SpellAgent", ()=> { return spellAgent; });
        var targetPort = AddValueOutPort("SpellTarget", () => { return spellAgent.SpellTargets; });
        AddFlowOut("Out");
    }

    public override void Started(SpellAgent sa)
    {
        spellAgent = sa;
        sa.SpellStartEvent += Run;
    }

    public override void Stoped(SpellAgent sa)
    {
        sa.SpellStartEvent -= Run;
    }

    void Run(SpellAgent sa)
    {
        this.GetFlowOut("Out").Call();
    }
}

public class TakeDamage : Node
{
    public override void RegisterPort()
    {
        base.RegisterPort();
        var target = this.AddValueInPort("target");
        var damage = this.AddValueInPort("damage");
        var o = this.AddFlowOut("out");
        this.AddFlowIn("In", () => { Invoke(target.Value as List<Unit>, (float)damage.Value); o.Call(); });
    }

    public void Invoke(List<Unit> target, float damage)
    {
        foreach (var unit in target)
        {
            unit.TakeDamage(damage);
        }
    }
}

