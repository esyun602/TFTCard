using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EventStageGenType
{
	StaticNumber = 0,
	Random = 1,
	StaticNumberSequence = 2,
}

public enum EventPoolType
{
	Sequence,
	RandomReplace,
	RandomNonReplace,
}

public abstract class StagePoolInfoBase
{
	protected List<string> stagePoolList;

	protected StagePoolInfoBase(List<string> stagePoolList)
	{
		this.stagePoolList = stagePoolList;
	}

	public virtual void Initialize()
	{
		
	}
	public abstract StageSpec GetStageSpec();
}

public class SequenceStagePoolInfo : StagePoolInfoBase
{
	private int idx;
	public override StageSpec GetStageSpec()
	{
		return GameDataSystem.Instance.GetGameData<StageData>().GetStageSpec(stagePoolList[idx++]);
	}

	public override void Initialize()
	{
		idx = 0;
	}

	public SequenceStagePoolInfo(List<string> stagePoolList) : base(stagePoolList)
	{
	}
}

public class ReplacementStagePoolInfo : StagePoolInfoBase
{
	public override StageSpec GetStageSpec()
	{
		return GameDataSystem.Instance.GetGameData<StageData>().GetStageSpec(stagePoolList.GetRandomElement());
	}

	public ReplacementStagePoolInfo(List<string> stagePoolList) : base(stagePoolList)
	{
	}
}
public class NonReplacementStagePoolInfo : StagePoolInfoBase
{
	private List<string> stagePoolListCopy;
	public override void Initialize()
	{
		stagePoolListCopy = new(stagePoolList);
	}

	public override StageSpec GetStageSpec()
	{
		return GameDataSystem.Instance.GetGameData<StageData>().GetStageSpec(stagePoolListCopy.GetAndRemoveRandomElement());
	}

	public NonReplacementStagePoolInfo(List<string> stagePoolList) : base(stagePoolList)
	{
	}
}

/// <summary>
/// 중복제거
/// </summary>
public class FlowGenSpec
{
	public string Name { get; private set; }
	public List<StageType> StageTypeOrder { get; private set; }
	public EventStageGenType EventStageGenType { get; private set; }
	/// <summary>
	/// random -> min, max
	/// static number -> count
	/// static number sequence -> count1, count2 ...
	/// </summary>
	public List<int> BranchParams { get; private set; }

	private Dictionary<StageType, StagePoolInfoBase> stagePoolDict;
	
	private int currentGeneratingIdx;
	
	public static FlowGenSpec Create(Dictionary<string, object> param)
	{
		var spec = new FlowGenSpec();
		spec.Name = param.GetString(nameof(Name));
		
		spec.StageTypeOrder = new();
		
		var stageTypes = param.GetStringArray(nameof(StageTypeOrder));
		foreach (var typeString in stageTypes)
		{
			spec.StageTypeOrder.Add(Enum.Parse<StageType>(typeString));
		}

		spec.EventStageGenType = Enum.Parse<EventStageGenType>(param.GetString(nameof(EventStageGenType)));

		spec.BranchParams = param.GetIntArray(nameof(BranchParams)).ToList();

		spec.stagePoolDict = new();
		foreach (StageType value in Enum.GetValues(typeof(StageType)))
		{
			var poolInfo = param.GetObject(value.ToString() + "Pool");
			if (poolInfo == null) continue;

			var poolType = Enum.Parse<EventPoolType>(poolInfo.GetString("PoolType"));
			switch (poolType)
			{
				case EventPoolType.Sequence:
					spec.stagePoolDict[value] = new SequenceStagePoolInfo(poolInfo.GetStringArray("PoolList").ToList());
					break;
				case EventPoolType.RandomReplace:
					spec.stagePoolDict[value] = new ReplacementStagePoolInfo(poolInfo.GetStringArray("PoolList").ToList());
					break;
				case EventPoolType.RandomNonReplace:
					spec.stagePoolDict[value] = new NonReplacementStagePoolInfo(poolInfo.GetStringArray("PoolList").ToList());
					break;
			}
		}

		return spec;
	}

	private void InitializePoolInfos()
	{
		foreach (var kvp in stagePoolDict)
		{
			kvp.Value.Initialize();
		}
	}
	
	public FlowInfo GenerateFlow()
	{
		InitializePoolInfos();
		currentGeneratingIdx = 0;
		var flowInfo = new FlowInfo(GameDataSystem.Instance.GetGameData<Constant>().MapEdgeCurveList.GetRandomElement());
		flowInfo.AddStartNodes(GenerateFlowNodes(StageTypeOrder[0]));
		
		for(var i = 1; i < StageTypeOrder.Count; i++)
		{
			currentGeneratingIdx++;
			var type = StageTypeOrder[i];

			var nodes = GenerateFlowNodes(type);
			LinkNodes(flowInfo.GetTails(), nodes);
		}

		return flowInfo;
	}

	//todo: event idx 수정
	private List<FlowNodeInfo> GenerateFlowNodes(StageType type)
	{
		if (type == StageType.EventStage)
		{
			var nodeCount = 0;
			if (EventStageGenType == EventStageGenType.StaticNumber)
			{
				nodeCount = BranchParams[0];
			}
			else if (EventStageGenType == EventStageGenType.StaticNumberSequence)
			{
				nodeCount = BranchParams[currentGeneratingIdx];
			}
			else if (EventStageGenType == EventStageGenType.Random)
			{
				var (min, max) = (BranchParams[0], BranchParams[1]);

				nodeCount = Random.Range(min, max + 1);
			}
			return GenerateMultipleNodeInfo(nodeCount);
		}
		else if (type == StageType.BattleStage)
		{
			var randomSpec = stagePoolDict[StageType.BattleStage].GetStageSpec();

			var node = new FlowNodeInfo(randomSpec, currentGeneratingIdx);
			return new List<FlowNodeInfo>(){ node };
		}

		return new();
	}

	private List<FlowNodeInfo> GenerateMultipleNodeInfo(int targetNum)
	{
		List<FlowNodeInfo> list = new();

		for (var i = 0; i < targetNum; i++)
		{
			var randomSpec = stagePoolDict[StageType.EventStage].GetStageSpec();
				
			list.Add(new FlowNodeInfo(randomSpec, currentGeneratingIdx));
		}
		
		return list;
	}

	private void LinkNodes(List<FlowNodeInfo> prevNodeList, List<FlowNodeInfo> list)
	{
		List<int> edgeCount = new();
		
		if (prevNodeList.Count > list.Count)
		{
			for (var i = 0; i < list.Count; i++)
			{
				edgeCount.Add(1);
			}
			
			var left = prevNodeList.Count - list.Count;
			for (var i = 0; i < left; i++)
			{
				edgeCount[Random.Range(0, edgeCount.Count)]++;
			}

			var total = 0;
			for (var i = 0; i < edgeCount.Count; i++)
			{
				for (var j = 0; j < edgeCount[i]; j++)
				{
					prevNodeList[total++].AddChild(list[i]);
				}
			}
		}
		else
		{
			for (var i = 0; i < prevNodeList.Count; i++)
			{
				edgeCount.Add(1);
			}
			
			var left = list.Count - prevNodeList.Count;
			for (var i = 0; i < left; i++)
			{
				edgeCount[Random.Range(0, edgeCount.Count)]++;
			}

			var total = 0;
			for (var i = 0; i < edgeCount.Count; i++)
			{
				for (var j = 0; j < edgeCount[i]; j++)
				{
					prevNodeList[i].AddChild(list[total++]);
				}
			}
		}
		
	}
}