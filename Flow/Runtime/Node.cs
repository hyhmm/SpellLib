using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace XFlow
{
    public partial class Node
    {
        public int ID;
        public Graph graph;
        protected Blackboard blackboard;
        protected Dictionary<string, ValueOut> portValueOutDict = new Dictionary<string, ValueOut>();
        protected Dictionary<string, ValueIn> portValueInDict = new Dictionary<string, ValueIn>();
        protected Dictionary<string, FlowIn> flowInDict = new Dictionary<string, FlowIn>();
        protected Dictionary<string, FlowOut> flowOutDict = new Dictionary<string, FlowOut>();

        public Dictionary<string, ValueOut> PortValueOutDict { get { return portValueOutDict; } private set { portValueOutDict = value; } }
        public Dictionary<string, ValueIn> PortValueInDict { get { return portValueInDict; } private set { portValueInDict = value; } }
        public Dictionary<string, FlowIn> FlowInDict { get { return flowInDict; } private set { flowInDict = value; } }
        public Dictionary<string, FlowOut> FlowOutDict { get { return flowOutDict; } private set { flowOutDict = value; } }

        public virtual void Init()
        {

        }

        public virtual void Destroy()
        {

        }

        public virtual void Load(Graph graph, SerNode sn)
        {
            this.graph = graph;
            this.blackboard = graph.Blackboard;
            this.ID = sn.ID;
            this.X = sn.X;
            this.Y = sn.Y;
            this.ExtraInfo = sn.ExtraInfo;

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


        public ValueOut AddValueOutPort(string name, ValueHandler valueHandler)
        {
            ValueOut pv = null;

            if (!portValueOutDict.TryGetValue(name, out pv))
            {
                pv = new ValueOut(this, name);
                portValueOutDict.Add(name, pv);
            }

            pv.SetValueHandler(valueHandler);
            return pv;
        }

        public ValueIn AddValueInPort(string name)
        {
            ValueIn pv;
            if (portValueInDict.TryGetValue(name, out pv))
            {
                return pv;
            }
            pv = new ValueIn(this, name);
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
            if (!flowInDict.TryGetValue(name, out fi))
            {
                fi = new FlowIn(this, name);
                flowInDict.Add(name, fi);
            }

            fi.SetFlowHandler(flowHandler);
            return fi;
        }

        public FlowOut AddFlowOut(string name)
        {
            FlowOut fo;
            if (flowOutDict.TryGetValue(name, out fo))
                return fo;

            fo = new FlowOut(this, name);
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
}
