using System.Collections.Generic;

public class SpellNode : Node
{
    protected SpellAgent spellAgent;
    public virtual void Init(SpellAgent sa)
    {
        spellAgent = sa;
    }
}

public class SpellStart : SpellNode
{
    public override void RegisterPort()
    {
        AddValueOutPort("SpellAgent", () => { return spellAgent; });
        AddValueOutPort("SpellTarget", () => { return spellAgent.SpellTargets; });
        AddFlowOut("Out");
    }

    public override void Init(SpellAgent sa)
    {
        base.Init(sa);
        sa.SpellStartEvent += Run;
    }

    void Run(SpellAgent sa)
    {
        this.GetFlowOut("Out").Call();
    }
}

public class TakeDamage : Node
{
    public override void RegisterPort()
    {
        base.RegisterPort();
        var target = this.AddValueInPort("target");
        var damage = this.AddValueInPort("damage");
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

