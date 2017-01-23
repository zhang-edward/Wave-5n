using UnityEngine;
using System.Collections;

public class CooldownCondition : EnemyCondition
{
	public float cooldown;
	[Tooltip("How much should be added to the cooldown timer when this entity is damaged")]
	public float interruptAmt = 0.5f;
	private float timer;

	public override void Init (Enemy e, Transform p)
	{
		base.Init (e, p);
		e.OnEnemyDamaged += Interrupt;
	}

	void Update()
	{
		timer -= Time.deltaTime;
	}

	private void Interrupt (int foo)
	{
		timer += interruptAmt;
	}

	public override bool Check ()
	{
		bool timerDone = timer <= 0;
		if (timerDone)
			timer = cooldown;
		return timerDone;
	}
}

