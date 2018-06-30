using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Pyro_Rage : HeroPowerUp {

	private const float MAX_RAGE_TIME = 10.0f;
	public GameObject rageMeterPrefab;

	private PyroRageMeter rageMeter;
	private PyroHero pyro;
	public float maxRageTimer;
	[HideInInspector] public float rage;

	public override void Activate(PlayerHero hero) {
		base.Activate(hero);
		pyro = (PyroHero)hero;
		pyro.OnPyroTeleportDamagedEnemy	+= ChargeRageSmallAmt;
		pyro.OnFireSpurtDamagedEnemy	+= ChargeRageSmallAmt;
		pyro.OnParriedEnemy += ChargeRageParry;

		// Instantiate rage meter
		GameObject o = Instantiate(rageMeterPrefab);
		o.transform.SetParent(BattleSceneManager.instance.gui.customUI, false);
		rageMeter = o.GetComponent<PyroRageMeter>();
		rageMeter.Init(this);
	}

	public override void Deactivate() {
		base.Deactivate();
		pyro.OnPyroTeleportDamagedEnemy -= ChargeRageSmallAmt;
		pyro.OnFireSpurtDamagedEnemy	-= ChargeRageSmallAmt;
		pyro.OnParriedEnemy -= ChargeRageParry;
		rageMeter.gameObject.SetActive(false);
	}

	private void ChargeRageSmallAmt() {
		ChargeRage(0.1f);
	}

	private void ChargeRageParry() {
		ChargeRage(0.2f);
	}

	private void ChargeRage(float amt) {
		rage = Mathf.Min(rage + amt, 1.0f);
		if (rage >= 1.0f && maxRageTimer <= 0)
			maxRageTimer = MAX_RAGE_TIME;
	}

	void Update() {
		UpdateRage();
		if (maxRageTimer > 0)
			pyro.cooldownMultipliers[0] = 0.75f;
		else
			pyro.cooldownMultipliers[0] = 1.0f;
	}

	private void UpdateRage() {
		// While MaxRage is active, don't decrease rage
		if (maxRageTimer > 0) {
			maxRageTimer -= Time.deltaTime;
			if (maxRageTimer <= 0) {
				maxRageTimer = 0;
				rage = 0;
			}
			return;
		}
		rage = Mathf.Max(rage - Time.deltaTime * 0.075f, 0);
	}
}
