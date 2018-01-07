﻿using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class CooldownCondition : EnemyCondition
	{
		public float initialTimerValue;
		public float cooldown;
		[Tooltip("How much should be added to the cooldown timer when this entity is damaged")]
		public float interruptAmt = 0.5f;
		private float timer;

		public override void Init(EnemyAction action, Enemy e, Transform p)
		{
			base.Init(action, e, p);
			e.OnEnemyDamaged += Interrupt;
			action.onExecute += ResetTimer;
			timer = initialTimerValue;
		}

		void Update()
		{
			if (timer > 0 && !e.hitDisabled)
				timer -= Time.deltaTime;
		}

		public override bool Check()
		{
			return timer <= 0;
		}

		private void Interrupt(int foo)
		{
			timer += interruptAmt;
		}

		private void ResetTimer()
		{
			timer = cooldown;
		}
	}
}

