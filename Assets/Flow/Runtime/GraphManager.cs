using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager {
    Dictionary<string, Graph> GraphDict = new Dictionary<string, Graph>();

    public Graph GetGraph(string graphName)
    {
        if (GraphDict.ContainsKey(graphName))
            return GraphDict[graphName];

        Graph graph = new Graph();
        graph.Load(graphName); ;
        this.GraphDict.Add(graphName, graph);
        return graph;

    }
}
