using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

public class GraphEditor : EditorWindow
{
    static Graph graph;

    [OnOpenAssetAttribute(0)]
    public static bool OnOpen(int instanceID, int line)
    {
        string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        if (path.EndsWith(".ue"))
        {
            var graph = new Graph();
            graph.LoadByFileName(path);
            OpenWindow(graph);
            return true;
        }
        return false;
    }

    public static GraphEditor OpenWindow(Graph graph)
    {
        var window = GetWindow<GraphEditor>();
        GraphEditor.graph = graph;
        return window;
    }

    private void OnGUI()
    {
    }
}
