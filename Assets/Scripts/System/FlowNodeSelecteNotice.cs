using MessageSystem;

public class FlowNodeSelectNotice : Notice
{
	public FlowNodeSelectNotice(FlowNodeInfo targetInfo)
	{
		TargetInfo = targetInfo;
	}

	public FlowNodeInfo TargetInfo { get; private set; }
	
}