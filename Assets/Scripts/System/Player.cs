using System.Collections.Generic;

public class Player
{
	public void Initialize()
	{
		//todo: load
		CurrentPlayInfo = new();
		CurrentPlayInfo.Initialize();
	}

	public void Dispose()
	{
		CurrentPlayInfo.Dispose();
		CurrentPlayInfo = null;
	}

	//todo: save infos
	public PlayInfo CurrentPlayInfo { get; private set; }
}