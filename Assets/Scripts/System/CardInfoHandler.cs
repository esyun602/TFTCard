using System;

public interface ICardInfoHandler
{
	public void Initialize(ICard card, IStat stat, Func<bool> isFxOn = null);
	public void Dispose();
}