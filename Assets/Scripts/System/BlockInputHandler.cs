using System.Collections.Generic;

public class BlockInputHandler
{
	private Dictionary<object, InputBlockFlag> blockRequestDict = new();
	
	public InputBlockFlag BlockInput { get; private set; }

	public bool IsBlocked(InputBlockFlag flag)
	{
		return (BlockInput & flag) != InputBlockFlag.None;
	}
		
	public void BlockInputs(InputBlockFlag flag, object requester)
	{
		if (!blockRequestDict.TryAdd(requester, flag))
		{
			blockRequestDict[requester] |= flag;
		}
		UpdateBlockFlags();
	}
	
	public void RestoreInputs(InputBlockFlag flag, object requester)
	{
		if (!blockRequestDict.ContainsKey(requester)) return;
		
		blockRequestDict[requester] &= ~flag;
		if (blockRequestDict[requester] == InputBlockFlag.None)
		{
			blockRequestDict.Remove(requester);
		}
		UpdateBlockFlags();
	}

	public bool HasRequest(object requester)
	{
		return blockRequestDict.ContainsKey(requester);
	}
	
	private void UpdateBlockFlags()
	{
		BlockInput = InputBlockFlag.None;
		foreach (var eachFlag in blockRequestDict.Values)
		{
			BlockInput |= eachFlag;
		}
	}
}