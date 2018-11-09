using System.Collections.Generic;

namespace XFlow
{
    #region event

    [Category("Spell/Event/SpellStart")]
    public class SpellStart : Node
    {
        public override void RegisterPort()
        {
            AddFlowOut("Out");
        }

        public override void Init()
        {
            var sa = graph.GraphOwner as SpellAgent;
            sa.SpellStartEvent += Invoke;
        }

        void Invoke(SpellAgent sa)
        {
            this.GetFlowOut("Out").Call();
        }
    }

    public class SpellAgentNode : Node
    {
        protected SpellAgent spellAgent;
        public override void RegisterPort()
        {
            AddValueOutPort("SpellAgent", () => { return spellAgent; });
            AddValueOutPort("SpellTarget", () => { return spellAgent.SpellTargets; });
            AddValueOutPort("FirePos", () => { return spellAgent.FirePos; });
        }
    }

    [Category("Spell/Event/ActionStart")]
    public class ActionStart : Node
    {
        public override void RegisterPort()
        {
            AddFlowOut("Out");
        }

        public override void Init()
        {
            var sa = graph.GraphOwner as SpellAgent;
            sa.SpellStartEvent += Invoke;
        }

        void Invoke(SpellAgent sa)
        {
            this.GetFlowOut("Out").Call();
        }
    }

    #endregion



    [Category("Spell/TakeDamage")]
    [Description("伤害方式来的范德萨")]
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

    public class IsNullOrEmpty : Node
    {
        FlowOut trueFlow;
        FlowOut falseFlow;
        public override void RegisterPort()
        {
            base.RegisterPort();
            var obj = AddValueInPort("Obj");

            trueFlow = this.AddFlowOut("True");
            falseFlow = this.AddFlowOut("False");
            this.AddFlowIn("In", () => { Invoke(obj); });
        }

        void Invoke(object obj)
        {
            if (obj == null)
                trueFlow.Call();
            else
                falseFlow.Call();

        }
    }
}

