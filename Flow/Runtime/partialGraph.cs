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
        public class Data
        {
            public string Name;
            public Variable Value;

            private string strValue = null;
            public string StrValue
            {
                get
                {
                    if (strValue == null)
                        strValue = Value.ToString();
                    return strValue;
                }
                set
                {
                    if (strValue == value)
                        return;

                    strValue = value;
                    Value.FromString(strValue);
                }
            }
        }

        List<Data> showDataList;
        public List<Data> ShowDataList
        {
            get
            {
                if (showDataList == null)
                {
                    showDataList = new List<Data>();
                    foreach (var itr in DataSource)
                    {
                        showDataList.Add(new Data()
                        {
                            Name = itr.Key,
                            Value = itr.Value
                        });
                    }
                }
                return showDataList;
            }
        }

        public void AddShowData()
        {
            ShowDataList.Add(new Data()
            {
                Name = GetNewName("New")
            });
        }

        string GetNewName(string name)
        {
            foreach (var data in this.ShowDataList)
            {
                if (data.Name == name)
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