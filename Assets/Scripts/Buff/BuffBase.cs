using MessageSystem;

public abstract class BuffBase : IBuff
{
	protected IBattleObject target;
	public abstract BuffType DefaultType { get; }
	public BuffType BuffType => DefaultType | additionalType;
	private BuffType additionalType;
	public void SetAdditionalType(BuffType type)
	{
		additionalType |= type;
	}

	public abstract UnitValueType ControlUnitValueType { get; }
	private int level;

	public int Level
	{
		get => level;
		set
		{
			level = value;
			if(target != null)
				NoticeSystem.Instance.Publish(new BuffLevelChangeNotice(this, target));
		}
	}

	public void AddTo(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Publish(new BuffAddNotice(target, this));
		OnAdd();
	}

	protected virtual void OnAdd()
	{
		
	}

	public void RemoveFromObject()
	{
		NoticeSystem.Instance.Publish(new BuffRemoveNotice(target, this));
		OnRemove();
	}

	protected virtual void OnRemove()
	{
		
	}

	public abstract bool TryStack(IBuff buff);

	public abstract string Keyword { get; }
}