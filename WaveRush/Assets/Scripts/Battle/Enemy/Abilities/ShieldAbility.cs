using UnityEngine;
using System.Collections;

public class ShieldAbility : EnemyAbility
{
	public Material outlineMat;
	public Material defaultMat;

	public bool shielded;
	public float cooldown;
	private float shieldTimer;

	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemy.OnEnemyDamaged += BreakShield;
	}

	void Update()
	{
		// shield timer
		shieldTimer -= Time.deltaTime;
		if (shieldTimer <= 0)
			shielded = true;
		// set material depending on whether shield is active
		if (shielded)
			enemy.sr.material = outlineMat;
		else
			enemy.sr.material = defaultMat;
	}

	private void BreakShield(int damage)
	{
		if (!shielded)
			return;
		//enemy.Heal(damage);
		shielded = false;
		shieldTimer = cooldown;
	}
}

