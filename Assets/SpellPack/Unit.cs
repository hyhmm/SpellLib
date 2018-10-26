using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour {

    public float HP;
    public event Action<Unit, float> OnTakeDamage;

    public void DispatchTakeDamage(float damage)
    {
        if (OnTakeDamage != null)
            OnTakeDamage(this, damage);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        DispatchTakeDamage(damage);

    }
}
