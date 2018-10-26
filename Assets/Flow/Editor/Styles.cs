using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFlow
{
    public static class Styles
    {

        private static GUIStyle _leftLabel;
        public static GUIStyle leftLabel
        {
            get
            {
                if (_leftLabel == null)
                {
                    _leftLabel = new GUIStyle(GUI.skin.GetStyle("label"));
                    _leftLabel.richText = true;
                    _leftLabel.alignment = TextAnchor.MiddleLeft;
                }
                return _leftLabel;
            }
        }

        private static GUIStyle _rightLabel;
        public static GUIStyle rightLabel
        {
            get
            {
                if (_rightLabel == null)
                {
                    _rightLabel = new GUIStyle(GUI.skin.GetStyle("label"));
                    _rightLabel.richText = true;
                    _rightLabel.alignment = TextAnchor.MiddleRight;
                }
                return _rightLabel;
            }
        }
    }
}
