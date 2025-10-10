using System.Runtime.CompilerServices;
using UnityEngine;

public static class Bootstrap
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Warmup()
	{
		RuntimeHelpers.RunClassConstructor(typeof(SkillValueType).TypeHandle);
		RuntimeHelpers.RunClassConstructor(typeof(UnitValueType).TypeHandle);
		RuntimeHelpers.RunClassConstructor(typeof(SkillValueType).TypeHandle);
	}
}