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
        public event Action OnKill;

        public ListUnitVariable SpellTargets;
        public Vector2Variable FirePos;
        public Graph Graph { get; private set; }
        public string GraphName;

        public void Awake()
        {
            Graph = new Graph();
            Graph.Owner = this;
            Graph.LoadByFileName(GraphName);
        }

        public void StartSkill()
        {
            DispatchSpellStart();
        }

        public void StartAction()
        {
            DispatchActionStart();
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

        public void DispatchOwnerDied()
        {
            if (OnOwnerDied != null)
                OnOwnerDied();
        }

        public void DispatchBulletHit()
        {
            if (OnBulletHit != null)
                OnBulletHit();
        }

        public void DispatchAttackLand()
        {
            if (OnAttackLand != null)
                OnAttackLand();
        }

        public bool IsReady()
        {
            BoolVariable isActive = this.Graph.Blackboard["Active"] as BoolVariable;
            if ((bool)isActive)
                return false;
            return true;
        }
    }
}
