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
        //Debug.Log("Load Graph");
        graph = new Graph();
        graph.Load(GraphName);
        this.Started();
    }

    private void OnDestroy()
    {
        this.Stoped();
    }

    public void Started()
    {
        graph.Started(this);
    }

    public void Stoped()
    {
        graph.Stoped(this);
    }

    public void DispatchSpellStart()
    {
        if (SpellStartEvent != null)
            SpellStartEvent(this);
    }
	
}
