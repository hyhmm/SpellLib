namespace XFlow
{
    public class GetVariable : Node
    {
        public string key;

        public override void RegisterPort()
        {
            AddValueOutPort("value", () => { return blackboard[key]; });
        }

        public override string ExtraInfo
        {
            get { return key; }
            set { key = value; }
        }

        public override string Name { get { return string.IsNullOrEmpty(key) ? "GetVariable" : "$" + key; } }
    }

    public class SetVariable : Node
    {
        public string key = "abc";
        public override void RegisterPort()
        {
            var v = AddValueInPort("value");
            var o = this.AddFlowOut("out");
            this.AddFlowIn("In", () => { blackboard[key] = v.Value; o.Call(); });
        }
        public override string ExtraInfo
        {
            get { return key; }
            set { key = value; }
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

    public class ConstantNode : Node
    {
        public string key = "";
 
        public override void RegisterPort()
        {
            base.RegisterPort();
            AddValueOutPort("value", () => { return key; });
        }

        public override string ExtraInfo
        {
            get { return key; }
            set { this.key = value; }
        }

        public override string Name { get { return string.IsNullOrEmpty(key) ? "ConstantNode" : key; } }
    }
}