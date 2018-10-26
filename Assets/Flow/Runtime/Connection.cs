
public partial class Connection
{
    public ConnectType connectType { get; set; }

    private Graph graph;

    public Node sourceNode { get; set; }

    public Port sourcePort { get; set; }

    public Node targetNode { get; set; }

    public Port targetPort { get; set; }

    public Connection(Graph graph)
    {
        this.graph = graph;
    }

    public void Connect(ConnectType connectType, int sourceId, string sourcePortName, int targetId, string targetPortName)
    {
        this.connectType = connectType;

        this.sourceNode = graph.GetNode(sourceId);
        this.targetNode = graph.GetNode(targetId);
        if (connectType == ConnectType.Flow)
        {
            sourcePort = sourceNode.GetFlowOut(sourcePortName);
            targetPort = targetNode.GetFlowIn(targetPortName);
        }
        else
        {
            sourcePort = sourceNode.GetValueOutPort(sourcePortName);
            targetPort = targetNode.GetValueInPort(targetPortName);
        }

        sourcePort.Connections.Add(this);
        targetPort.Connections.Add(this);
    }

    public object Value
    {
        get
        {
            if (connectType != ConnectType.Value)
                return null;

            if (sourcePort == null)
                return null;

            if (!(sourcePort is ValueOut))
                return null;

            return (sourcePort as ValueOut).Value;
        }
    }

    public void Call()
    {
        if (connectType != ConnectType.Flow)
            return;

        if (targetPort == null)
            return;

        if (!(targetPort is FlowIn))
            return;

        (targetPort as FlowIn).Call();
    }
}