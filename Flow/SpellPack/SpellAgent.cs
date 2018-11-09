using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XFlow
{
    public class SpellAgent : GraphOwner
    {
        public event Action<SpellAgent> SpellStartEvent;
        public event Action<SpellAgent> ActionStartEvent;
        public List<Unit> SpellTargets;
        public Vector2 FirePos;
        Graph graph;
        public string GraphName;

        public void Start()
        {
            graph = new Graph();
            graph.Load(GraphName);
        }

        public void DispatchSpellStart()
        {
            if (SpellStartEvent != null)
                SpellStartEvent(this);
        }

        public void DispatchActionStart()
        {
            if (ActionStartEvent != null)
                ActionStartEvent(this);
        }
    }
}
