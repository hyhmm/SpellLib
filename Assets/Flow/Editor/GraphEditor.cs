using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.IO;
using Newtonsoft.Json;

public class GraphEditor : EditorWindow
{
    static SerGraph serGraph;

    [OnOpenAssetAttribute(0)]
    public static bool OnOpen(int instanceID, int line)
    {
        string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        if (path.EndsWith(".ue"))
        {
            path = Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, path);
            string data = FileUtil.ReadTextFile(path);
            serGraph = JsonConvert.DeserializeObject<SerGraph>(data);
            GetWindow<GraphEditor>();
            return true;
        }
        return false;
    }

    private void OnGUI()
    {
        if (serGraph == null)
            return;

        DrawGrid();
        DrawNodes();
        DrawConnections();
        DrawBlackboard();
    }

    void DrawNodes()
    {
        BeginWindows();
        foreach (var node in serGraph.Nodes)
        {
            DrawNode(node);
        }
        EndWindows();
    }

    void DrawNode(SerNode node)
    {
        GUI.color = Color.white;
        var rect = GUILayout.Window(node.ID, new Rect(node.X, node.Y, node.DefaultWidth, node.DefaultHeight),(id) => { NodeWindowGUI(id, node); }, string.Empty, CanvasStyles.window);
        node.Rect = rect;
        GUI.Box(rect, string.Empty, CanvasStyles.windowShadow);
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.Box(new Rect(rect.x + 6, rect.y + 6, rect.width, rect.height), string.Empty, CanvasStyles.windowShadow);
    }

    void DrawConnections()
    {
        const float startOffset = 35;
        const float portHeight = 18;

        Dictionary<SerPort, Vector2> PortPos = new Dictionary<SerPort, Vector2>();
        foreach (var node in serGraph.Nodes)
        {
            int flowCount = node.FlowCount;
            if (node.FlowIn != null)
            {
                for (int i = 0; i < node.FlowIn.Count; i++)
                {
                    Rect rect = new Rect(0, 0, 10, 10);
                    rect.position = new Vector2(node.X-15, node.Y + i* portHeight + startOffset);
                    GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                    PortPos.Add(new SerPort() { NodeId = node.ID, Port = node.FlowIn[i] }, rect.position);
                }
            }

            if (node.FlowOut != null)
            {
                for (int i = 0; i < node.FlowOut.Count; i++)
                {
                    Rect rect = new Rect(0, 0, 10, 10);
                    rect.position = new Vector2(node.X + node.Width+5, node.Y + i * portHeight + startOffset);
                    GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                    PortPos.Add(new SerPort() { NodeId = node.ID, Port = node.FlowOut[i] }, rect.position);
                }
            }

            if (node.ValueIn != null)
            {
                for (int i = 0; i < node.ValueIn.Count; i++)
                {
                    Rect rect = new Rect(0, 0, 10, 10);
                    rect.position = new Vector2(node.X - 15, node.Y + (i + flowCount) * portHeight + startOffset);
                    GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                    PortPos.Add(new SerPort() { NodeId = node.ID, Port = node.ValueIn[i] }, rect.position);
                }
            }

            if (node.ValueOut != null)
            {
                for (int i = 0; i < node.ValueOut.Count; i++)
                {
                    Rect rect = new Rect(0, 0, 10, 10);
                    rect.position = new Vector2(node.X + node.Width + 5, node.Y + (i + flowCount) * portHeight + startOffset);
                    GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                    PortPos.Add(new SerPort() { NodeId = node.ID, Port = node.ValueOut[i] }, rect.position);
                }
            }
        }

        foreach (var connect in serGraph.Connections)
        {
            if (connect.Source == 0)
                continue;
            var startPos = PortPos[new SerPort() { NodeId = connect.Source, Port = connect.SourcePort }] + new Vector2(5, 5);
            var endPos = PortPos[new SerPort() { NodeId = connect.Target, Port = connect.TargetPort }] + new Vector2(5, 5);

            var xDiff = (startPos.x - endPos.x) * 0.8f;
            xDiff = startPos.x > endPos.x ? xDiff : -xDiff;
            var tangA = new Vector2(xDiff, 0);
            var tangB = tangA * -1;

            Color color = connect.Type == ConnectType.Flow ? new Color(0.5f, 0.5f, 0.8f, 0.8f) : new Color(0.3f, 0.3f, 0.3f, 1f);
            Handles.DrawBezier(startPos, endPos, startPos + tangA, endPos + tangB, color, null, 3);
        }
    }

    void NodeWindowGUI(int id, SerNode node)
    {
        var text = string.Format("<b><size=12><color=#{0}>{1}</color></size></b>", "eed9a7", node.Type);
        var content = new GUIContent(text, "node");
        GUILayout.Label(content, CanvasStyles.nodeTitle, GUILayout.MaxHeight(23));

        int flowCount = node.FlowCount;
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                for (int i = 0; i < flowCount; i++)
                {
                    if (node.FlowIn != null && i < flowCount)
                        GUILayout.Label(string.Format("<b>► {0}</b>", node.FlowIn[i]), Styles.leftLabel);
                    else
                        GUILayout.Label("");
                }

                if (node.ValueIn != null)
                {
                    foreach (var valueIn in node.ValueIn)
                    {
                        GUILayout.Label(valueIn, Styles.leftLabel);
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical();
            {
                for (int i = 0; i < flowCount; i++)
                {
                    if (node.FlowOut != null && i < node.FlowOut.Count)
                        GUILayout.Label(string.Format("<b>{0} ►</b>", node.FlowOut[i]), Styles.rightLabel);
                    else
                        GUILayout.Label("");
                }
                if (node.ValueOut != null)
                {
                    foreach (var valueOut in node.ValueOut)
                    {
                        Debug.Log(valueOut);
                        GUILayout.Label(valueOut, Styles.rightLabel);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

    }

    void DrawFlowIn(string flowIn)
    {
      
    }

    private static GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true), GUILayout.Height(16) };
    void DrawBlackboard()
    {
        SerBlackboard serBlackboard = serGraph.Blackboard;
        Rect rect = new Rect(position.width - 300, 30, 280, 300);
        GUI.Box(rect, string.Empty, CanvasStyles.windowShadow);
        GUI.color = Color.white;
        GUILayout.BeginArea(rect, "Variable", CanvasStyles.editorPanel);
        GUILayout.Space(5);

        GUI.skin.label.richText = true;
        var e = Event.current;
        GUI.backgroundColor = new Color(0.8f, 0.8f, 1);
        if (GUILayout.Button("Add Variable"))
        {
            Event.current.Use();
        }
        GUI.backgroundColor = Color.white;

        if (serBlackboard.Values.Count == 0)
        {
            EditorGUILayout.HelpBox("Blackboard has no variables", MessageType.Info);
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label("Name", layoutOptions);
            GUILayout.Label("Value", layoutOptions);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }

        //EditorUtils.EndOfInspector();
        GUILayout.EndArea();
    }

    void DrawGrid()
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        Handles.color = new Color(0, 0, 0, 0.15f);

        var drawGridSize = 15;
        var step = drawGridSize;

        var xStart = 0;
        var xEnd = position.width-0;
        for (var i = xStart; i < xEnd; i += step)
        {
            Handles.DrawLine(new Vector3(i, 0, 0), new Vector3(i, position.height, 0));
        }

        var yStart = 0;
        var yEnd = position.height;
        for (var i = yStart; i < yEnd; i += step)
        {
            Handles.DrawLine(new Vector3(0, i, 0), new Vector3(position.width, i, 0));
        }
    }
}
