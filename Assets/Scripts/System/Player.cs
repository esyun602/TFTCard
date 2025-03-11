using System.Collections.Generic;

public class Player
{
	public void Initialize()
	{
		//todo: load
		CurrentPlayInfo = new();
		CurrentPlayInfo.Initialize();
	}

	//todo: save infos
	public PlayInfo CurrentPlayInfo { get; private set; }
}