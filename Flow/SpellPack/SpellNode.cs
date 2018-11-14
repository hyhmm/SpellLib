using System.Collections.Generic;

namespace XFlow
{
    #region event

    [Category("Spell/Event/OnSpellStart")]
    public class OnSpellStart : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            o = AddFlowOut("Out");
        }

        public override void Init()
        {
            var sa = graph.Owner as SpellAgent;
            sa.OnSpellStart += ()=> o.Call();
        }
    }

    [Category("Spell/SpellAgent")]
    public class SpellAgentNode : Node
    {
        protected SpellAgent spellAgent;
        public override void RegisterPort()
        {
            spellAgent = graph.Owner as SpellAgent;
            AddValueOutPort("SpellAgent", () => { return spellAgent; });
            AddValueOutPort("SpellTarget", () => { return spellAgent.SpellTargets; });
            AddValueOutPort("FirePos", () => { return spellAgent.FirePos; });
        }
    }

    [Category("Spell/Event/ActionStart")]
    public class OnActionStart : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            o = AddFlowOut("Out");
        }

        public override void Init()
        {
            var sa = graph.Owner as SpellAgent;
            sa.OnSpellStart += () => o.Call();
        }
    }


    #region t1
    public class OnUpdate : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            base.RegisterPort();
            this.AddValueInPort("UpdateInterval");
            o = this.AddFlowOut("Out");
            //timer.Timeout(dt, Update);
        }

        void Update()
        {
            o.Call();
            //time.Timeout(dt, Update);
        }
        
    }

    public class OnCreate : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            base.RegisterPort();
            o = AddFlowOut("Out");
        }

        public override void Init()
        {
            o.Call();
        }
    }

    public class OnAttackLand : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            base.RegisterPort();
            o = AddFlowOut("Out");
        }

        public override void Init()
        {
            SpellAgent sa = graph.Owner as SpellAgent;
            sa.OnAttackLand += () => o.Call();
        }
    }

    public class OnDestroy : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            base.RegisterPort();
            o = AddFlowOut("Out");
        }

        public override void Destroy()
        {
            base.Destroy();
            o.Call();
        }
    }

    public class OnKill : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            base.RegisterPort();
            o = AddFlowOut("Out");
        }

        public override void Init()
        {
            base.Init();
            SpellAgent sa = graph.Owner as SpellAgent;
            sa.OnKill += () => o.Call();
        }
    }

    #endregion

    #endregion
    

    public class AddAbility : Node
    {

    }

    public class AttachEffect : Node
    {

    }


    [Category("Spell/TakeDamage")]
    public class TakeDamage : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var target = this.AddValueInPort("Target");
            var damage = this.AddValueInPort("Damage");
            var o = this.AddFlowOut("out");
            this.AddFlowIn("In", () => { Invoke(target.Value as List<Unit>, (float)damage.Value); o.Call(); });
        }

        public void Invoke(List<Unit> target, float damage)
        {
            foreach (var unit in target)
            {
                unit.TakeDamage(damage);
            }
        }
    }

    public class SpawnTower : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var towerId = AddValueInPort("TowerId");
            var duration = AddValueInPort("Duration");
            var attackRate = AddValueInPort("AttackRate");
            var spawnPos = AddValueInPort("SpawnPos");
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke((int)towerId.Value, (int)duration.Value, (int)attackRate.Value, (UnityEngine.Vector2)spawnPos.Value); o.Call(); });
        }

        public void Invoke(int towerId, int duration, int attackRate, UnityEngine.Vector2 position)
        {

        }
    }

    public class AddBuff : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var targets = AddValueInPort("Targets");
            var buffId = AddValueInPort("BuffId");
            var duration = AddValueInPort("Duration");
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(targets.Value, (int)buffId.Value, (int)duration.Value); o.Call(); });
        }

        public void Invoke(object targets, int buffId, int duration)
        {

        }
    }

    public class FindOptionTarget : Node
    {
        List<Unit> Targets;
        public override void RegisterPort()
        {
            base.RegisterPort();
            var optionTargets = AddValueInPort("OptionTargets");
            var tags = AddValueInPort("Tags");
            var teams = AddValueInPort("Teams");
            var exceptTargs = AddValueInPort("ExceptTargs");
            var target = AddValueOutPort("Targets", () => { return Targets; });
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(optionTargets, tags, teams, exceptTargs); o.Call(); });
        }

        public void Invoke(object optionTargets, object tags, object teams, object exceptTargets)
        {

        }
    }

    public class FindTargetInCircle : Node
    {
        List<Unit> Targets;
        public override void RegisterPort()
        {
            base.RegisterPort();
            var center = AddValueInPort("Center");
            var range = AddValueInPort("Range");
            var tags = AddValueInPort("Tags");
            var teams = AddValueInPort("Teams");
            var exceptTargs = AddValueInPort("ExceptTargs");
            var count = AddValueInPort("Count");
            var target = AddValueOutPort("Targets", () => { return Targets; });
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(center, tags, teams, exceptTargs); o.Call(); });
        }

        public void Invoke(object optionTargets, object tags, object teams, object exceptTargets)
        {

        }
    }

    public class FireEffect : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var position = AddValueInPort("Position");
            var effectName = AddValueInPort("EffectName");
       
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(position, effectName); o.Call(); });
        }

        public void Invoke(object position, object effectName)
        {

        }
    }

    public class Delay : Node
    {
        FlowOut o;
        public override void RegisterPort()
        {
            base.RegisterPort();
            var time = AddValueInPort("Time");

            o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(time); });
        }

        public void Invoke(object time)
        {
            // wait time
            o.Call();
        }
    }

   
}

