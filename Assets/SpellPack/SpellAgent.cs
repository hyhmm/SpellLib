using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpellAgent : MonoBehaviour {
    public event Action<SpellAgent> SpellStartEvent;
    public List<Unit> SpellTargets;
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
	
}
