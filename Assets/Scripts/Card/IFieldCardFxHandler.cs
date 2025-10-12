public interface IFieldCardFxHandler
{
	public bool ActivateFx { get; }
	public void Initialize();
	public void Dispose();
}