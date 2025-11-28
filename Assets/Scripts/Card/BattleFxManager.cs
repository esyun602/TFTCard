using System;
using System.Collections.Generic;
using MessageSystem;
using Unity.Mathematics;

public class BattleFxManager : IUpdatable
{
	private class FxInfo : IUpdatable
	{
		private float fxInterval;
		private float timePassed;
		private Queue<(UnityObjectPool, string)> buffFxQueue;
		private IBattleObject bo;

		public FxInfo(IBattleObject bo, float fxInterval = 0.5f)
		{
			this.fxInterval = fxInterval;
			this.bo = bo;
			buffFxQueue = new();
			timePassed = 1f;
		}

		public void AddQueueBuff(IBuff buff)
		{
			var keywordInfo = GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword(buff.Keyword);
			UnityObjectPool pool = null;
			string sfxName = null;
			if (buff.Level > 0)
			{
				pool = UnityObjectPool.GetOrCreatePool("Fx",
					keywordInfo.PoolName, disposeTime: 5f);
				sfxName = keywordInfo.SfxName;
			}
			else if(buff.Level < 0)
			{
				pool = UnityObjectPool.GetOrCreatePool("Fx",
					keywordInfo.ReducePoolName, disposeTime: 5f);
				sfxName = keywordInfo.ReduceSfxName;
			}
			
			if (pool == null) return;
			
			AddQueueFx(pool, sfxName);
		}
		public void AddQueueFx(UnityObjectPool pool, string sfxName)
		{
			buffFxQueue.Enqueue((pool, sfxName));
		}

		public void AddShield()
		{
			var keywordInfo = GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword("Shield");
			AddQueueFx(UnityObjectPool.GetOrCreatePool("Fx", keywordInfo.PoolName, disposeTime: 5f), keywordInfo.SfxName);
		}
		
		public void UpdateFrame(float dt)
		{
			timePassed += dt;
			if (buffFxQueue.Count == 0 || bo == null || bo.IsDead())
			{
				return;
			}
			
			if (timePassed > fxInterval)
			{
				timePassed = 0f;
				var (info, sfxName) = buffFxQueue.Dequeue();
				info.Instantiate(bo.Position, quaternion.identity, followTarget: bo.Transform);
				SfxManager.Instance.PlayAt(sfxName, bo.Position, true);
			}
		}
	}
	
	private Dictionary<IBattleObject, FxInfo> fxDict;
	public void Initialize()
	{
		fxDict = new();
		//todo: 나중에 분리, register로 일괄
		NoticeSystem.Instance.Subscribe<BuffAddNotice>(OnBuffAdd);
		NoticeSystem.Instance.Subscribe<BuffStackSuccessNotice>(OnBuffStackNotice);
		NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
	}

	private void OnBattleValueChange(UnitBattleValueChangeNotice m)
	{
		if (m.Type != UnitValueType.Shield || m.Diff <= 0) return;
		
		if (fxDict.TryGetValue(m.Stat.Owner, out var info))
		{
			info.AddShield();
		}
		else
		{
			fxDict[m.Stat.Owner] = new FxInfo(m.Stat.Owner);
			fxDict[m.Stat.Owner].AddShield();
		}
	}

	public void RegisterFx(IBattleObject bo, UnityObjectPool pool, string sfxName = null)
	{
		if (fxDict.TryGetValue(bo, out var info))
		{
			info.AddQueueFx(pool, sfxName);
		}
		else
		{
			fxDict[bo] = new FxInfo(bo);
			fxDict[bo].AddQueueFx(pool, sfxName);
		}
		
	}


	private void OnBuffAdd(BuffAddNotice m)
	{
		if (String.IsNullOrEmpty(m.Buff.Keyword))
		{
			return;
		}
		
		if (fxDict.TryGetValue(m.Target, out var info))
		{
			info.AddQueueBuff(m.Buff);
		}
		else
		{
			fxDict[m.Target] = new FxInfo(m.Target);
			fxDict[m.Target].AddQueueBuff(m.Buff);
		}
	}

	private void OnBuffStackNotice(BuffStackSuccessNotice m)
	{
		if (String.IsNullOrEmpty(m.StackedBuff.Keyword))
		{
			return;
		}
		
		if (fxDict.TryGetValue(m.Target, out var info))
		{
			info.AddQueueBuff(m.StackedBuff);
		}
		else
		{
			fxDict[m.Target] = new FxInfo(m.Target);
			fxDict[m.Target].AddQueueBuff(m.StackedBuff);
		}
	}

	private void OnBuffRemove(BuffRemoveNotice m)
	{
	}

	public void UpdateFrame(float dt)
	{
		foreach (var kvp in fxDict)
		{
			kvp.Value.UpdateFrame(dt);
		}
	}
	
	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<BuffAddNotice>(OnBuffAdd);
		NoticeSystem.Instance.Unsubscribe<BuffStackSuccessNotice>(OnBuffStackNotice);
		NoticeSystem.Instance.Unsubscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
	}

}