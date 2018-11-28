using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace XFlow
{
    public partial class Graph
    {
        public string Name;
        public Blackboard Blackboard { get; private set; }
        Dictionary<int, Node> nodes = new Dictionary<int, Node>();
        public Dictionary<int, Node> Nodes { get { return nodes; } private set { nodes = value; } }
        public List<Connection> Connections = new List<Connection>();
        public GraphOwner Owner;
        public void LoadByFileName(string fileName)
        {
            this.Name = Path.GetFileName(fileName);
            TextAsset ta = Resources.Load<TextAsset>(fileName);
            Load(ta.text);
            Init();
        }

        void Init()
        {
            foreach (var node in Nodes.Values)
                node.Init();
        }

        public void Load(SerGraph sg)
        {
            // 加载blackboard
            if (sg.Blackboard != null)
            {
                Blackboard = new Blackboard();
                Blackboard.Load(sg.Blackboard);
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
        }

        public void Load(string content)
        {
            Blackboard = new Blackboard();
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            SerGraph sg = JsonConvert.DeserializeObject<SerGraph>(content);

            Load(sg);
        }

        public Node GetNode(int id)
        { 
            if (nodes.ContainsKey(id))
                return nodes[id];

            Debug.LogErrorFormat("cant find node by id:{0}", id);
            return null;
        }
    }
}