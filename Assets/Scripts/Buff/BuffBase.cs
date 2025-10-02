using MessageSystem;

public abstract class BuffBase : IBuff
{
	protected IBattleObject target;
	public abstract BuffType DefaultType { get; }
	public BuffType BuffType => DefaultType | additionalType;
	private BuffType additionalType;
	private UnityObjectPool pool;
	public void SetAdditionalType(BuffType type)
	{
		additionalType |= type;
	}

	protected BuffBase()
	{
		pool = UnityObjectPool.GetOrCreatePool("Fx", GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword(Keyword)
			.PoolName, disposeTime: 2f);
	}

	public abstract UnitValueType ControlUnitValueType { get; }
	private int level;

	public int Level
	{
		get => level;
		set
		{
			var diff = value - level;
			level = value;
			if(target != null)
				NoticeSystem.Instance.Publish(new BuffLevelChangeNotice(this, target, diff));
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

	public bool TryStack(IBuff buff)
	{
		if (buff.GetType() != GetType()) return false;
		
		if (TryStackImpl(buff))
		{
			NoticeSystem.Instance.Publish(new BuffStackSuccessNotice(this, buff, target));
			return true;
		}
		else
		{
			NoticeSystem.Instance.Publish(new BuffStackFailNotice(this, buff, target));
			return false;
		}
	}

	protected abstract bool TryStackImpl(IBuff buff);

	public abstract string Keyword { get; }
}