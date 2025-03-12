
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessageSystem;

public class TurnSystem
{
	//todo: to balanced bst?
	private class TurnOrderHandler
	{
		public class TurnOrderNode : IEnumerable
		{
			public TurnOrderNode left;
			public TurnOrderNode right;
			public ITurnObject target;
			public TurnOrderNode(ITurnObject target)
			{
				this.target = target;
			}
			
			public void AddChildNode(TurnOrderNode child)
			{
				if (child.target.TurnSpeed > target.TurnSpeed)
				{
					if (left == null)
					{
						left = child;
					}
					else
					{
						left.AddChildNode(child);
					}
				}
				else
				{
					if (right == null)
					{
						right = child;
					}
					else
					{
						right.AddChildNode(child);
					}
				}
			}

			public void RemoveChildNode(ITurnObject obj)
			{
				if (target.TurnSpeed < obj.TurnSpeed)
				{
					if (left.target == obj)
					{
						RLReplace(ref left);
					}
					else
					{
						left.RemoveChildNode(obj);
					}
				}
				else
				{
					if (right.target == obj)
					{
						RLReplace(ref right);
					}
					else
					{
						right.RemoveChildNode(obj);
					}
				}
			}

			public void RLReplace(ref TurnOrderNode targetPtr)
			{
				if (targetPtr.right == null)
				{
					targetPtr = targetPtr.left;
				}
				else
				{
					var replace = targetPtr.right;
					if (replace.left == null)
					{
						targetPtr.right = replace.right;
					}
					else
					{
						while (replace.left != null)
						{
							var tmp = replace.left;
							if (tmp.left == null)
							{
								replace.left = tmp.right;
							}
							replace = tmp;
						}
					}

					replace.right = targetPtr.right;
					replace.left = targetPtr.left;
					targetPtr = replace;
				}
			}

			public IEnumerator GetEnumerator()
			{
				if(left != null)
				{
					foreach (var lTarget in left)
					{
						yield return lTarget;
					}
				}

				yield return target;

				if (right != null)
				{
					foreach (var rTarget in right)
					{
						yield return rTarget;
					}
				}
			}
		}

		private TurnOrderNode root;

		public void AddObj(ITurnObject turnObject)
		{
			var child = new TurnOrderNode(turnObject);
			if (root == null)
			{
				root = child;
			}
			else
			{
				root.AddChildNode(child);
			}
		}

		public void RemoveObj(ITurnObject turnObject)
		{
			if (turnObject == root.target)
			{
				root.RLReplace(ref root);
			}
			else
			{
				root.RemoveChildNode(turnObject);
			}
		}
		
		public IEnumerator GetEnumerator()
		{
			if (root == null) yield break;
			foreach (var node in root)
			{
				yield return node;
			}
		}
	}
	private const float MaxTurnGauge = 100;
	private TurnOrderHandler turnOrderHandler;
	private ITurnObject currentObject;
	//todo: 타이 해결
	private Queue<ITurnObject> candidates;
	private Action<float> currentUpdateRoutine;
	private IUpdatableRoutine priorityRoutine;
	private IEnumerator currentTurnEnumerator;
	private PlayerTurn playerTurn;
	
	public void Initialize()
	{
		//todo: fix subscribe once
		NoticeSystem.Instance.Subscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);
		
		turnOrderHandler = new();
		candidates = new();
		
		playerTurn = new PlayerTurn();
		playerTurn.Initialize();
	}

	private void OnBattleStageInitRoutineDone(BattleStageInitRoutineDoneNotice m)
	{
		//todo: 일일히 해줘야하나?
		playerTurn.StartTurn();
		currentUpdateRoutine = UpdatePlayerTurn;
	}

	public void InitializeAutoTurn()
	{
		// ReSharper disable once NotDisposedResource : No Dispose Needed
		currentTurnEnumerator = turnOrderHandler.GetEnumerator();
		if (!currentTurnEnumerator.MoveNext())
		{
			//todo: fix
			playerTurn.StartTurn();
			currentUpdateRoutine = UpdatePlayerTurn;
			return;
		}
		
		currentObject = (ITurnObject)currentTurnEnumerator.Current;
		currentObject.StartTurn();
	}

	public void Dispose()
	{
		(currentTurnEnumerator as IDisposable)?.Dispose();
		playerTurn.Dispose();
		NoticeSystem.Instance.Unsubscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);
	}

	public void UpdateTurn(float dt)
	{
		if (priorityRoutine != null)
		{
			priorityRoutine.UpdateFrame(dt, out var done);
			if (done)
			{
				priorityRoutine = null;
			}

			return;
		}
		currentUpdateRoutine?.Invoke(dt);
	}

	private void UpdatePlayerTurn(float dt)
	{
		playerTurn.UpdatableCurrentRoutine.UpdateFrame(dt, out var done);
		if (done)
		{
			InitializeAutoTurn();
			currentUpdateRoutine = UpdateAutoTurn;
		}
	}

	private void UpdateAutoTurn(float dt)
	{
		//todo: start 전에 update가 불리는 경우 방지
		currentObject.UpdatableRoutine.UpdateFrame(dt, out var routineDone);
		if (routineDone)
		{
			NoticeSystem.Instance.Publish(new TurnEndNotice(currentObject));
			if (currentTurnEnumerator.MoveNext())
			{
				currentObject = (ITurnObject)currentTurnEnumerator.Current;
				currentObject.StartTurn();
				NoticeSystem.Instance.Publish(new TurnStartNotice(currentObject));
			}
			else
			{
				(currentTurnEnumerator as IDisposable)?.Dispose();
				playerTurn.StartTurn();
				currentUpdateRoutine = UpdatePlayerTurn;
			}
		}
	}
	
	public void RegisterNewObject(ITurnObject obj, float startGauge = 0f)
	{
		turnOrderHandler.AddObj(obj);
		
		NoticeSystem.Instance.Publish(new TurnObjectRegisterNotice(obj));
	}

	public void UnregisterObject(ITurnObject obj)
	{
		turnOrderHandler.RemoveObj(obj);
		
		NoticeSystem.Instance.Publish(new TurnObjectUnregisterNotice(obj));
	}
	
	//todo: fix?
	public void RegisterPriorityRoutine(IUpdatableRoutine routine)
	{
		priorityRoutine = routine;
	}
	
	//턴?
	//이동/소환 -> 턴종 --> 플레이어 턴도 speed를 가지게?
}