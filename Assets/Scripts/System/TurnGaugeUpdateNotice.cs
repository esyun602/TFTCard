using System.Collections.Generic;
using MessageSystem;

public class TurnGaugeUpdateNotice : Notice
{
	//todo: 수정 못하게
	public float MaxGauge { get; }
	public Dictionary<ITurnObject, float> GaugeInfoDictionary { get; }

	public TurnGaugeUpdateNotice(float maxGauge, Dictionary<ITurnObject, float> gaugeInfoDictionary)
	{
		MaxGauge = maxGauge;
		GaugeInfoDictionary = gaugeInfoDictionary;
	}
}