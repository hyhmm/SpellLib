using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdateHp : MonoBehaviour {
    public Unit unit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Text>().text = unit.HP.ToString();
	}
}
