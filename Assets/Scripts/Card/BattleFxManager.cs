using System.Collections.Generic;
using MessageSystem;
using Unity.Mathematics;

public class BattleFxManager : IUpdatable
{
	private class BuffFxInfo : IUpdatable
	{
		private float fxInterval;
		private float timePassed;
		private Queue<UnityObjectPool> buffFxQueue;
		private IBattleObject bo;

		public BuffFxInfo(IBattleObject bo, float fxInterval = 0.5f)
		{
			this.fxInterval = fxInterval;
			this.bo = bo;
			buffFxQueue = new();
			timePassed = 0;
		}

		public void AddQueue(IBuff buff)
		{
			var keywordInfo = GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword(buff.Keyword);
			if (buffFxQueue.Count == 0) timePassed = 1f;
			buffFxQueue.Enqueue(UnityObjectPool.GetOrCreatePool("Fx", keywordInfo.PoolName, disposeTime: 5f));
		}
		
		public void UpdateFrame(float dt)
		{
			if (buffFxQueue.Count == 0 || bo == null || bo.IsDead())
			{
				return;
			}
			
			timePassed += dt;
			if (timePassed > fxInterval)
			{
				timePassed = 0f;
				var info = buffFxQueue.Dequeue();
				info.Instantiate(bo.Position, quaternion.identity, followTarget: bo.Transform);
			}
		}
	}
	
	private Dictionary<IBattleObject, BuffFxInfo> buffFxDict;
	public void Initialize()
	{
		buffFxDict = new();
		NoticeSystem.Instance.Subscribe<BuffAddNotice>(OnBuffAdd);
		NoticeSystem.Instance.Subscribe<BuffStackSuccessNotice>(OnBuffStackNotice);
	}


	private void OnBuffAdd(BuffAddNotice m)
	{
		if (buffFxDict.TryGetValue(m.Target, out var info))
		{
			info.AddQueue(m.Buff);
		}
		else
		{
			buffFxDict[m.Target] = new BuffFxInfo(m.Target);
			buffFxDict[m.Target].AddQueue(m.Buff);
		}
	}

	private void OnBuffStackNotice(BuffStackSuccessNotice m)
	{
		if (buffFxDict.TryGetValue(m.Target, out var info))
		{
			info.AddQueue(m.StackedBuff);
		}
		else
		{
			buffFxDict[m.Target] = new BuffFxInfo(m.Target);
			buffFxDict[m.Target].AddQueue(m.StackedBuff);
		}
	}

	private void OnBuffRemove(BuffRemoveNotice m)
	{
	}

	public void UpdateFrame(float dt)
	{
		foreach (var kvp in buffFxDict)
		{
			kvp.Value.UpdateFrame(dt);
		}
	}
	
	public void Dispose()
	{
		NoticeSystem.Instance.Subscribe<BuffAddNotice>(OnBuffAdd);
		NoticeSystem.Instance.Subscribe<BuffStackSuccessNotice>(OnBuffStackNotice);
	}

}