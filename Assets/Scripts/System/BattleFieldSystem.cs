using System.Collections.Generic;
using System.Linq;
using MessageSystem;

public class BattleFieldSystem
{
	private Dictionary<ObjectType, List<IBattleObject>> battleObjectDict;

	public void Initialize()
	{
		battleObjectDict = new();
		NoticeSystem.Instance.Subscribe<BattleObjectDestroyedNotice>(OnDestroy);
	}

	public void Register(IBattleObject targetObject)
	{
		if (!battleObjectDict.TryGetValue(targetObject.ObjectType, out var list))
		{
			list = new List<IBattleObject> { targetObject };
			battleObjectDict[targetObject.ObjectType] = list;
		}
		else
		{
			list.Add(targetObject);
		}
	}

	public void UnRegister(IBattleObject targetObject, IUpdatableRoutine context)
	{
		battleObjectDict[targetObject.ObjectType].Remove(targetObject);
		if (battleObjectDict[targetObject.ObjectType].Count == 0)
		{
			NoticeSystem.Instance.Publish(new BattleObjectTypeEliminateNotice(targetObject.ObjectType, context));
		}
	}

	public List<IBattleObject> GetAllObjectOfType(ObjectType type)
	{
		return battleObjectDict.GetValueOrDefault(type);
	}
	
	public List<IBattleObject> GetAllObject()
	{
		var merged =
			battleObjectDict.Values
				.SelectMany(list => list)
				.ToList();
		return merged;
	}


	private void OnDestroy(BattleObjectDestroyedNotice m)
	{
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<BattleObjectDestroyedNotice>(OnDestroy);
	}
}