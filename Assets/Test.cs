using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

public class Test : MonoBehaviour {

    [MenuItem("Tools/Test")]
    public static void T1()
    {
        /*
        SerValue sv = new SerValue();
        sv.Name = "Damage";
        sv.Value = new List<int>(){ 1, 23,3};
        Debug.Log(JsonConvert.SerializeObject(sv));
        SerValue sv1 = JsonConvert.DeserializeObject<SerValue>("{\"Name\":\"Damage\",\"Value\":[1,23,3]}");
        Debug.Log(sv1.Value.GetType());
        foreach (var v in (JArray)sv1.Value)
        {
            Debug.Log(v);
        }
        Debug.Log(((JArray)sv1.Value)[1]);
        */
        Graph graph = new Graph();
        graph.LoadByFileName("attack");
        graph.Save();
    }
}
