public interface IOption
{
	public void OnAdd(IBattleObject target);
	public void OnRemove();
	public int Level { get; set; }
}