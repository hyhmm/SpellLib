using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XFlow
{
    partial class Port
    {
        public void ClearConnections()
        {
            for (int i = Connections.Count - 1; i >= 0; i--)
            {
                var connection = Connections[i];
                connection.sourcePort.Connections.Remove(connection);
                connection.targetPort.Connections.Remove(connection);
                this.node.graph.Connections.Remove(connection);
            }
        }
    }

    partial class Node
    {
        public int X;
        public int Y;

        public int DrawOrder;

        public float Width = 100f;

        public float Height = 50;

        public virtual string ExtraInfo { get { return null; } set { } }

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

        public int DefaultWidth { get { return 100; } }
        public int DefaultHeight
        {
            get
            {
                return (FlowCount + PortCount) * 30;
            }
        }

        public int FlowCount
        {
            get { return System.Math.Max(FlowInDict.Count, FlowOutDict.Count); }
        }

        public int PortCount
        {
            get { return System.Math.Max(PortValueInDict.Count, PortValueOutDict.Count); }
        }

        public void Load(Graph graph)
        {
            this.graph = graph;
            this.blackboard = graph.Blackboard;
            this.ID = graph.GenNewId();

            this.RegisterPort();
        }

        public void RemoveConnection()
        {
            FlowInDict.Values.ToList().ForEach((x) => x.ClearConnections());
            FlowOutDict.Values.ToList().ForEach((x) => x.ClearConnections());
            PortValueInDict.Values.ToList().ForEach((x) => x.ClearConnections());
            PortValueOutDict.Values.ToList().ForEach((x) => x.ClearConnections());

        }

        public virtual void OnNodeInspectorGUI()
        {
            EditorUtils.ReflectedObjectInspector(this);
        }

        public virtual string Name
        {
            get { return this.GetType().ToString().Split('.').Last(); }
        }

    }

    partial class Blackboard
    {
        public List<string> DataKeyList = new List<string>();

        public void AddVariable(VariableType vt)
        {
            string name = GetNewName("default");
            this.AddVariable(GetNewName("default"), vt);
            DataKeyList.Add(name);
        }

        public void RemoveVariable(string name)
        {
            dataSource.Remove(name);
            DataKeyList.Remove(name);
        }

        public void Rename(string oldName, string newName)
        {
            int idx = DataKeyList.FindIndex((x) => x == oldName);
            DataKeyList[idx] = newName;

            var v = dataSource[oldName];
            dataSource[newName] = v;
        }

        public void AddVariable(string name, VariableType vt)
        {
            Variable v = null;
            switch (vt)
            {
                case VariableType.Int:
                    v = new IntVariable();
                    break;
                case VariableType.Float:
                    v = new FloatVariable();
                    break;
                case VariableType.ListInt:
                    v = new ListIntVariable();
                    break;
                case VariableType.ListFloat:
                    v = new ListFloatVariable();
                    break;
                case VariableType.String:
                    v = new StringVariable();
                    break;
                default:
                    break;
            }

            AddData(name, v);
        }

        string GetNewName(string name)
        {
            foreach (var key in this.DataSource.Keys)
            {
                if (key == name)
                {
                    return GetNewName(name + "1");
                }
            }
            return name;
        }
    }

    partial class Graph
    {
        public void CreateConnection(Port port1, Port port2)
        {
            Connection connection = new Connection(this);

            Port source, target;
            if (port1 is FlowIn || port1 is FlowOut)
            {
                connection.connectType = ConnectType.Flow;

                if (port1 is FlowOut)
                {
                    source = port1;
                    target = port2;
                }
                else
                {
                    source = port2;
                    target = port1;
                }
            }
            else
            {
                connection.connectType = ConnectType.Value;

                if (port1 is ValueOut)
                {
                    source = port1;
                    target = port2;
                }
                else
                {
                    source = port2;
                    target = port1;
                }
            }

            connection.sourcePort = source;
            connection.sourceNode = source.node;
            connection.targetPort = target;
            connection.targetNode = target.node;
            Connections.Add(connection);
            source.Connections.Add(connection);
            target.Connections.Add(connection);
        }

        public Node CopyNode(Node node)
        {
            return AddNode(node.GetType());
        }

        public Node AddNode(Type nodeType)
        {
            var clone = (Node)Activator.CreateInstance(nodeType);
            clone.Load(this);
            nodes.Add(clone.ID, clone);
            return clone;
        }

        public int GenNewId()
        {
            int id = 1;
            while (nodes.Keys.Contains(id))
                id++;
            return id;
        }

        public void RemoveNode(Node node)
        {
            this.nodes.Remove(node.ID);
            node.RemoveConnection();
        }
    }

    public static class NodeEditorUtilities
    {
        public static bool GetAttrib<T>(Type classType, out T attribOut) where T : Attribute
        {
            object[] attribs = classType.GetCustomAttributes(typeof(T), false);
            return GetAttrib(attribs, out attribOut);
        }

        public static bool GetAttrib<T>(object[] attribs, out T attribOut) where T : Attribute
        {
            for (int i = 0; i < attribs.Length; i++)
            {
                if (attribs[i].GetType() == typeof(T))
                {
                    attribOut = attribs[i] as T;
                    return true;
                }
            }
            attribOut = null;
            return false;
        }

        public static bool GetAttrib<T>(Type classType, string fieldName, out T attribOut) where T : Attribute
        {
            object[] attribs = classType.GetField(fieldName).GetCustomAttributes(typeof(T), false);
            return GetAttrib(attribs, out attribOut);
        }

        public static bool HasAttrib<T>(object[] attribs) where T : Attribute
        {
            for (int i = 0; i < attribs.Length; i++)
            {
                if (attribs[i].GetType() == typeof(T))
                {
                    return true;
                }
            }
            return false;
        }
    }
}