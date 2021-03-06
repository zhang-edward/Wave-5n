﻿namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;

	[System.Serializable]
	public class PA_Teleport : PlayerAction
	{
		/** Properties */
		#pragma warning disable 0649
		[SerializeField] private bool disableInput = true;
		[SerializeField] private float teleportOutTime	 = 0;			// How long the player should take to teleport out
		[SerializeField] private float teleportInTime	 = 0;			// How long the player should take to teleport in
		[SerializeField] private string teleportOutState = "Default";
		[SerializeField] private string teleportInState  = "Default";
		[SerializeField] private SimpleAnimation teleportOutEffect;
		[SerializeField] private TempObjectInfo teleportOutEffectProperties;
		[SerializeField] private SimpleAnimation teleportInEffect;
		[SerializeField] private TempObjectInfo teleportInEffectProperties;
		#pragma warning restore 0649
		
		public AudioClip teleportOutSound;
		public AudioClip teleportInSound;
		public bool invincibleDuringTeleport;

		private SoundManager sound;
		private Vector2 destination;

		public delegate void TeleportEvent();
		public event TeleportEvent OnTeleportOut;
		public event TeleportEvent OnTeleportIn;

		public override void Init(Player player)
		{
			base.Init(player);
			sound = SoundManager.instance;
		}

		protected override void DoAction()
		{
			player.StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine()
		{
			/** Set player properties */
			if (invincibleDuringTeleport)
				player.invincibility.Add(teleportOutTime + teleportInTime);

			/** Teleport out */
			if (teleportOutEffect.frames.Length > 0)
				EffectPooler.PlayEffect(teleportOutEffect, hero.transform.position, teleportOutEffectProperties);
			/** Animation */
			if (teleportOutState != "")
			{
				hero.anim.GetAnimation(teleportOutState).SetTimeLength(teleportOutTime);
				hero.anim.Play(teleportOutState);
			}
			if (disableInput)
				player.inputDisabled.Add(duration);
			sound.RandomizeSFX(teleportOutSound);
			if (OnTeleportOut != null)
				OnTeleportOut();

			yield return new WaitForSeconds(teleportOutTime);
			player.transform.parent.position = destination;

			/** Teleport In */
			if (teleportInEffect.frames.Length > 0)
				EffectPooler.PlayEffect(teleportInEffect, hero.transform.position, teleportInEffectProperties);
			/** Animation */
			if (teleportInState != "") 
			{
				hero.anim.GetAnimation(teleportInState).SetTimeLength(teleportInTime);
				hero.anim.Play(teleportInState);
			}
			sound.RandomizeSFX(teleportInSound);
			if (OnTeleportIn != null)
				OnTeleportIn();
		}


		public void SetDestination(Vector2 destination)
		{
			this.destination = destination;
		}
	}
}
