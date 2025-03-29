using UnityEngine;

[CreateAssetMenu]
public class TestTwoSynergyActionSpec : SynergyActionSpec
{
	public override ISynergyInstance Create()
	{
		return new TestTwoSynergy();
	}
}