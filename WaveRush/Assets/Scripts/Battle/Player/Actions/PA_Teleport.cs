namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;

	[System.Serializable]
	public class PA_Teleport : PlayerAction
	{
		/** Properties */
		[SerializeField] private float teleportOutTime	 = 0;			// How long the player should take to teleport out
		[SerializeField] private float teleportInTime	 = 0;			// How long the player should take to teleport in
		[SerializeField] private string teleportOutState = "Default";
		[SerializeField] private string teleportInState  = "Default";
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
			/** Set animation lengths */
			hero.anim.GetAnimation(teleportOutState).SetTimeLength(teleportOutTime);
			hero.anim.GetAnimation(teleportInState).SetTimeLength(teleportInTime);

			/** Set player properties */
			if (invincibleDuringTeleport)
				player.invincibility.Add(teleportOutTime + teleportInTime);

			/** Teleport out */
			hero.anim.Play(teleportOutState);
			player.input.isInputEnabled = false;
			sound.RandomizeSFX(teleportOutSound);
			if (OnTeleportOut != null)
				OnTeleportOut();

			yield return new WaitForSeconds(teleportOutTime);
			player.transform.parent.position = (Vector3)player.dir + player.transform.parent.position;

			/** Teleport In */
			hero.anim.Play(teleportInState);
			player.input.isInputEnabled = true;
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
