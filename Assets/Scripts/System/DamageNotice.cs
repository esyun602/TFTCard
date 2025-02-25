using MessageSystem;

public class DamageNotice : ContextNotice
{
	public IBattleObject Sender { get; }
	public IBattleObject Target { get; }
	public int Damage { get; }

	//todo: context 값을 어떻게 할 건지 고민 필요, UpdatableRoutine.CurrentRoutine 가져다 쓸 경우
	//루틴 외부에서 호출할 경우 context가 null이 되는데 이러면 루틴 관련 처리가 불가해짐
	//-> 이게 문제긴 한가? --> 문제가 될 수 있는게, 애초에 카드 트리거 시점에 호출하는 것도 루틴 외부 호출임
	//루틴 내부에서 호출하도록 강제할 수도 없음.
	//Damage 자체를 루틴으로 ? -> AddCoroutine 추가?
	//유저가 이걸 호출할 때 근데 Damage루틴만 종료됐다고 트리거 되는 걸 바라는게 아닐텐데
	//애초에 시퀀스가 보장되어야 하는 루틴을 위해 만든거라 코루틴은 에러
	//진짜 코루틴을 관찰하며 끝나는 걸 방지하는 역할만 한ㄴ건 괜찮을수도
	public DamageNotice(IBattleObject sender, IBattleObject target, int damage)
	{
		Sender = sender;
		Target = target;
		Damage = damage;
	}
}