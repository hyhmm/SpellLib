using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XFlow
{
    public class SpellAgent : GraphOwner
    {
        public event Action OnSpellStart;
        public event Action OnActionStart;
        public event Action OnOwnerDied;
        public event Action OnBulletHit;
        public event Action OnAttackLand;

        public List<Unit> SpellTargets;
        public Vector2 FirePos;
        Graph graph;
        public string GraphName;

        public void Start()
        {
            graph = new Graph();
            graph.LoadByFileName(GraphName);
        }

        public void DispatchSpellStart()
        {
            if (OnSpellStart != null)
                OnSpellStart();
        }

        public void DispatchActionStart()
        {
            if (OnActionStart != null)
                OnActionStart();
        }
    }
}
