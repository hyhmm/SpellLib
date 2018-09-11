using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft;
using Newtonsoft.Json.Linq;

public class Test : MonoBehaviour {

    [MenuItem("Tools/Test")]
    public static void T1()
    {
        Debug.Log("T1");

        Graph graph = new Graph();
        graph.Load("shiled");
    }
}
