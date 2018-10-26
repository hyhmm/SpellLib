using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseSpell : MonoBehaviour {


	// Use this for initialization
	void Start () {
        var bt = this.GetComponent<Button>();
        bt.onClick.AddListener(() => { this.GetComponent<XFlow.SpellAgent>().DispatchSpellStart(); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
