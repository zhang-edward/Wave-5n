namespace PlayerActions
{
	using UnityEngine;

	[System.Serializable]
	public class PA_Effect : PlayerAction
	{
		/** Set in Inspector */
		public SimpleAnimation effect;

		private TempObject lastPlayedEffect;

		/** Properties */
		protected enum RotationType
		{
			None,                   // Rotation is Quaternion.identity
			Set,					// Rotation is set by SetRotation method
			MatchFlipX,             // Match the player sprite's flipX
			ReverseFlipX,           // Match the reverse of the player sprite's flipX
			Random,					// Rotation is random
		}
		[SerializeField] protected RotationType rotationType = RotationType.None; // Default value doesn't matter, just prevents warning
		[SerializeField] protected Color color = Color.white;
		protected Quaternion rotation;
		private Vector3 position;

		public override void Init(Player player) {
			base.Init(player);
		}

		public void SetPosition(Vector3 pos) {
			position = pos;
		}

		public void SetRotation(Vector3 dir) {
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}

		public TempObject GetLastPlayedEffect()	{
			return lastPlayedEffect;
		}

		protected override void DoAction()
		{
			// Set the rotation for the effect
			if (rotationType != RotationType.Set)
				rotation = GetRotation();

			// Initialize the effect properties
			TempObjectInfo info = new TempObjectInfo();
			info.targetColor = color;
			if (duration < 0)
				duration = effect.TimeLength;
			info.lifeTime = duration;
			info.fadeOutTime = 0.1f;

			lastPlayedEffect = EffectPooler.PlayEffect(effect, position, info, rotation);
		}

		protected Quaternion GetRotation()
		{
			float angle;
			switch (rotationType)
			{
				case RotationType.MatchFlipX:
					angle = player.sr.flipX ? 180 : 0;
					break;
				case RotationType.ReverseFlipX:
					angle = player.sr.flipX ? 0 : 180;
					break;
				case RotationType.Random:
					angle = Random.Range(0, 360);
					break;
				default:
					angle = 0;
					break;
			}
			return Quaternion.Euler(new Vector3(0, 0, angle));
		}
	}
}
