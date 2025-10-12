using MessageSystem;

public class EnemyCardFxHandler : IFieldCardFxHandler
{
	public UnitCardInField Owner { get; private set; }
	public EnemyCardFxHandler(UnitCardInField owner)
	{
		Owner = owner;
	}


	public bool ActivateFx { get; private set; }
	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<EnemyIconHoverNotice>(OnHover);
		NoticeSystem.Instance.Subscribe<EnemyIconRemoveHoverNotice>(OnRemoveHover);
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<EnemyIconHoverNotice>(OnHover);
		NoticeSystem.Instance.Unsubscribe<EnemyIconRemoveHoverNotice>(OnRemoveHover);
	}

	private void OnRemoveHover(EnemyIconRemoveHoverNotice m)
	{
		if (m.Target.Stat.Owner == Owner)
		{
			ActivateFx = false;
		}
	}

	private void OnHover(EnemyIconHoverNotice m)
	{
		if (m.Target.Stat.Owner == Owner)
		{
			ActivateFx = true;
		}
	}
}