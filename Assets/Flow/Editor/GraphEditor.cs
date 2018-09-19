using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class GraphEditor : EditorWindow
{
    private static GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true), GUILayout.Height(16) };

    Dictionary<Port, Rect> PortPos = new Dictionary<Port, Rect>();

    static Graph graph;

    static string filePath;

    [OnOpenAssetAttribute(0)]
    public static bool OnOpen(int instanceID, int line)
    {
        string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        if (path.EndsWith(".ue"))
        {
            filePath = Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, path);
            string data = Util.ReadTextFile(path);
            SerGraph sg = JsonConvert.DeserializeObject<SerGraph>(data);
            graph = new Graph();
            graph.Load(sg);
            GetWindow<GraphEditor>();
            return true;
        }
        return false;
    }

    private void OnGUI()
    {
        if (graph == null)
            return;

        DrawGrid();
        DrawNodes();
        DrawConnections();
        DrawBlackboard();
        DrawToolbar();
    }


    #region Node
    void DrawNodes()
    {
        BeginWindows();
        foreach (var node in graph.Nodes.Values)
        {
            DrawNode(node);
        }
        EndWindows();
    }

    void DrawNode(Node node)
    {
        GUI.color = Color.white;
        var rect = GUILayout.Window(node.ID, new Rect(node.X, node.Y, node.DefaultWidth, node.DefaultHeight),(id) => { NodeWindowGUI(id, node); }, string.Empty, CanvasStyles.window);
        node.Rect = rect;
        GUI.Box(rect, string.Empty, CanvasStyles.windowShadow);
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.Box(new Rect(rect.x + 6, rect.y + 6, rect.width, rect.height), string.Empty, CanvasStyles.windowShadow);
    }

    void NodeWindowGUI(int id, Node node)
    {
        var text = string.Format("<b><size=12><color=#{0}>{1}</color></size></b>", "eed9a7", node.GetType().ToString());
        var content = new GUIContent(text, "node");
        GUILayout.Label(content, CanvasStyles.nodeTitle, GUILayout.MaxHeight(23));

        int flowCount = node.FlowCount;
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                // flowIn
                var flowIns = node.FlowInDict.Values.ToArray();
                for (int i = 0; i < flowCount; i++)
                {
                    if (i < flowIns.Count())
                        GUILayout.Label(string.Format("<b>► {0}</b>", flowIns[i].name), Styles.leftLabel);
                    else
                        GUILayout.Label("");
                }

                // valueIn
                foreach (var valueIn in node.PortValueInDict.Values)
                {
                    GUILayout.Label(valueIn.name, Styles.leftLabel);
                }
            }
                GUILayout.EndVertical();
        }

        GUILayout.BeginVertical();
        {
            // flowOut
            var flowOuts = node.PortValueOutDict.Values.ToArray();
            for (int i = 0; i < flowCount; i++)
            {
                if ( i < flowOuts.Count())
                    GUILayout.Label(string.Format("<b>{0} ►</b>", flowOuts[i].name), Styles.rightLabel);
                else
                    GUILayout.Label("");
            }

            // valueOut
            foreach (var valueOut in node.PortValueOutDict.Values)
            {
                GUILayout.Label(valueOut.name, Styles.rightLabel);
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    #endregion

    #region Connection
    void DrawConnections()
    {
        const float startOffset = 35;
        const float portHeight = 18;

        PortPos.Clear();

        foreach (var node in graph.Nodes.Values)
        {
            int flowCount = node.FlowCount;

            var flowIns = node.FlowInDict.Values.ToArray();
            for (int i = 0; i < flowIns.Count(); i++)
            {
                Rect rect = new Rect(0, 0, 10, 10);
                rect.position = new Vector2(node.X - 15, node.Y + i * portHeight + startOffset);
                GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                PortPos.Add(flowIns[i], rect);
            }

            var flowOuts = node.FlowOutDict.Values.ToArray();
            for (int i = 0; i < flowOuts.Count(); i++)
            {
                Rect rect = new Rect(0, 0, 10, 10);
                rect.position = new Vector2(node.X + node.Width + 5, node.Y + i * portHeight + startOffset);
                GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                PortPos.Add(flowOuts[i], rect);
            }

            var valueIns = node.PortValueInDict.Values.ToArray();
            for (int i = 0; i < valueIns.Count(); i++)
            {
                Rect rect = new Rect(0, 0, 10, 10);
                rect.position = new Vector2(node.X - 15, node.Y + (i + flowCount) * portHeight + startOffset);
                GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                PortPos.Add(valueIns[i], rect);
            }

            var valueOuts = node.PortValueOutDict.Values.ToArray();
            for (int i = 0; i < valueOuts.Count(); i++)
            {
                Rect rect = new Rect(0, 0, 10, 10);
                rect.position = new Vector2(node.X + node.Width + 5, node.Y + (i + flowCount) * portHeight + startOffset);
                GUI.Box(rect, string.Empty, CanvasStyles.nodePortConnected);
                PortPos.Add(valueOuts[i], rect);
            }
        }

        foreach (var connect in graph.Connections)
        {
            var startPos = PortPos[connect.sourcePort].center;
            var endPos = PortPos[connect.targetPort].center;

            var xDiff = (startPos.x - endPos.x) * 0.8f;
            xDiff = startPos.x > endPos.x ? xDiff : -xDiff;
            var tangA = new Vector2(xDiff, 0);
            var tangB = tangA * -1;

            Color color = connect.connectType == ConnectType.Flow ? new Color(0.5f, 0.5f, 0.8f, 0.8f) : new Color(0.3f, 0.3f, 0.3f, 1f);
            Handles.DrawBezier(startPos, endPos, startPos + tangA, endPos + tangB, color, null, 3);
        }
    }
    #endregion

    #region Blackboard
    void DrawBlackboard()
    {
        Blackboard blackboard = graph.Blackboard;
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
            blackboard.AddShowData();
            Event.current.Use();
        }
        GUI.backgroundColor = Color.white;

        if (blackboard.DataSource.Count == 0)
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
        
            GUILayout.BeginVertical();

            for (int i = blackboard.ShowDataList.Count - 1; i >= 0; i--)
            {
                var data = blackboard.ShowDataList[i];
                GUILayout.BeginHorizontal();

                data.Name = EditorGUILayout.DelayedTextField(data.Name, layoutOptions);

                data.StrValue = EditorGUILayout.DelayedTextField(data.StrValue, layoutOptions);

                if (GUILayout.Button("-"))
                {
                    blackboard.ShowDataList.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            
        }
        
        GUILayout.EndArea();
    }
    #endregion

    #region Background
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
    #endregion

    #region Toolbar
    void DrawToolbar()
    {
        if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            string data = graph.Serialize();
            Util.WriteTextFile(filePath, data);
        }
    }
    #endregion

}
