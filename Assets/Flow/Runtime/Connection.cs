
public class Connection
{
    ConnectType connectType;

    private Graph graph;

    Node sourceNode;

    Port sourcePort;

    Node targetNode;

    Port targetPort;

    public Connection(Graph graph)
    {
        this.graph = graph;
    }

    public void Connect(ConnectType connectType, int sourceId, string sourcePortName, int targetId, string targetPortName)
    {
        this.connectType = connectType;

        this.sourceNode = graph.GetNode(sourceId);
        this.targetNode = graph.GetNode(targetId);
        sourcePort = sourceNode.GetFlowOut(sourcePortName);
        targetPort = targetNode.GetFlowIn(targetPortName);

        sourcePort.Connection = this;
        targetPort.Connection = this;
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