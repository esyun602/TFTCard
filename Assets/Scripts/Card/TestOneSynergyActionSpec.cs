using UnityEngine;

[CreateAssetMenu]
public class TestOneSynergyActionSpec : SynergyActionSpec
{
	public override ISynergyInstance Create()
	{
		return new TestOneSynergy();
	}
}