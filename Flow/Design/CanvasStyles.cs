using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XFlow
{
    [InitializeOnLoad]
    public static class CanvasStyles
    {
        private static GUISkin styleSheet;

        [InitializeOnLoadMethod]
        static void Load()
        {
            styleSheet = Resources.Load<GUISkin>("StyleSheetDark");
        }
        private static GUIStyle _nodeTitle;
        public static GUIStyle nodeTitle
        {
            get
            {
                if (_nodeTitle == null)
                {
                    _nodeTitle = new GUIStyle();
                    _nodeTitle.margin = new RectOffset(4, 4, 4, 4);
                    _nodeTitle.padding = new RectOffset(0, 0, 3, 3);
                    _nodeTitle.alignment = TextAnchor.MiddleCenter;
                    _nodeTitle.richText = true;
                }
                return _nodeTitle;
            }
        }

        private static GUIStyle _windowShadow;
        public static GUIStyle windowShadow
        {
            get { return _windowShadow ?? (_windowShadow = styleSheet.GetStyle("windowShadow")); }
        }

        public static GUIStyle window
        {
            get { return styleSheet.window; }
        }

        private static GUIStyle _nodePortConnected;
        public static GUIStyle nodePortConnected
        {
            get { return _nodePortConnected ?? (_nodePortConnected = styleSheet.GetStyle("nodePortConnected")); }
        }

        private static GUIStyle _nodePortEmpty;
        public static GUIStyle nodePortEmpty
        {
            get { return _nodePortEmpty ?? (_nodePortEmpty = styleSheet.GetStyle("nodePortEmpty")); }
        }

        private static GUIStyle _editorPanel;
        public static GUIStyle editorPanel
        {
            get { return _editorPanel ?? (_editorPanel = styleSheet.GetStyle("editorPanel")); }
        }

        public static GUIStyle box
        {
            get { return styleSheet.box; }
        }

        public static GUIStyle button
        {
            get { return styleSheet.button; }
        }
    }
}