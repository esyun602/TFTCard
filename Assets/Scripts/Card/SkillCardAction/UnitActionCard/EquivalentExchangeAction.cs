using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EquivalentExchangeAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;

	public EquivalentExchangeAction(EquivalentExchangeActionSpec spec) : base(spec)
	{
	}
	public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (canceled)
		{
			routineDone = true;
			return;
		}

		routineDone = false;

		timePassed += dt;
		if (timePassed > 0f)
		{
			//todo: owner가 있으면 그냥 스탯에 owner 스탯을 합쳐버리는 방향으로 수정
			var handLimit = Constant.PlayerHandMax;
			var toDraw = handLimit - Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.PlayerHand.CardList.Count;

			for (var i = 0; i < toDraw; i++)
			{
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawPlayerCard();
			}

			BattleStat.Owner.Damage(new DamageInfo()
			{
				Sender = BattleStat.Owner,
				Dmg = toDraw
			});
			
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}