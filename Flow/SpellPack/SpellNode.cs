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
        protected SpellAgentVariable spellAgent;
        public override void RegisterPort()
        {
            spellAgent = graph.Owner as SpellAgent;
            AddValueOutPort("SpellAgent", () => { return spellAgent; });
            AddValueOutPort("SpellTarget", () => { return spellAgent.Value.SpellTargets; });
            AddValueOutPort("FirePos", () => { return spellAgent.Value.FirePos; });
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

    [Category("Spell/Event/OnUpdate")]
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

    [Category("Spell/Event/OnCreate")]
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

    [Category("Spell/Event/OnAttackLand")]
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

    [Category("Spell/Event/OnDestroy")]
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

    [Category("Spell/Event/OnKill")]
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

    [Category("Spell/Action/AddAbility")]
    public class AddAbility : Node
    {

    }

    [Category("Spell/Action/AttachEffect")]
    public class AttachEffect : Node
    {

    }


    [Category("Spell/Action/TakeDamage")]
    public class TakeDamage : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var target = this.AddValueInPort("Target");
            var damage = this.AddValueInPort("Damage");
            var o = this.AddFlowOut("out");
            this.AddFlowIn("In", () => {/* Invoke(target.Value as List<Unit>, (float)damage.Value);*/ o.Call(); });
        }

        public void Invoke(List<Unit> target, float damage)
        {
            foreach (var unit in target)
            {
                unit.TakeDamage(damage);
            }
        }
    }

    [Category("Spell/Action/SpawnTower")]
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
            this.AddFlowIn("In", () => { Invoke(towerId.Value as IntVariable, duration.Value as IntVariable, attackRate.Value as IntVariable, 
               spawnPos.Value as Vector2Variable); o.Call(); });
        }

        public void Invoke(IntVariable towerId, IntVariable duration, IntVariable attackRate, Vector2Variable position)
        {

        }
    }

    [Category("Spell/Action/AddBuff")]
    public class AddBuff : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var targets = AddValueInPort("Targets");
            var buffId = AddValueInPort("BuffId");
            var duration = AddValueInPort("Duration");
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(targets.Value as ListUnitVariable, buffId.Value as IntVariable, duration.Value as FloatVariable); o.Call(); });
        }

        public void Invoke(ListUnitVariable targets, IntVariable buffId, FloatVariable duration)
        {

        }
    }

    [Category("Spell/Action/FindOptionTarget")]
    public class FindOptionTarget : Node
    {
        ListUnitVariable Targets;
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

    public class PlayAnim : Node
    {
        public override void RegisterPort()
        {
            base.RegisterPort();
            var animName = AddValueInPort("AnimName");
            var o = this.AddFlowOut("Out");
            this.AddFlowIn("In", () => { Invoke(animName.Value); o.Call(); });
        }

        void Invoke(object anim)
        {
            var animation = graph.Owner.GetComponent<UnityEngine.Animation>();
            animation.Play((string)anim);
            animation.CrossFadeQueued("stand", 0.2f, UnityEngine.QueueMode.CompleteOthers);

        }
    }

    [Category("Spell/Action/FindTargetInCircle")]
    public class FindTargetInCircle : Node
    {
        ListUnitVariable Targets;
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

    [Category("Spell/Action/FireEffect")]
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

    [Category("Spell/Action/Delay")]
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

