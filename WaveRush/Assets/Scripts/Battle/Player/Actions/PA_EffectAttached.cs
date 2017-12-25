namespace PlayerActions
{
	using UnityEngine;

	[System.Serializable]
	public class PA_EffectAttached : PlayerAction
	{
		/** Set in Inspector */
		public SimpleAnimationPlayer anim;

		/** Properties */
		private enum RotationType {
			None,					// Rotation is Quaternion.identity
			Same,					// Rotation matches player dir
			Opposite,				// Rotation is reverse player dir
			MatchFlipX,				// Match the player sprite's flipX
			ReverseFlipX			// Match the reverse of the player sprite's flipX
		}
		[SerializeField] private RotationType rotationType = RotationType.None;	// Default value doesn't matter, just prevents warning
		[SerializeField] private bool 		  offsetMatchesFlipX = false;
		[SerializeField] private Color		  color = Color.white;

		public override void Init(Player player)
		{
			base.Init(player);
		}

		protected override void DoAction()
		{
			if (offsetMatchesFlipX)
			{
				int sign = offsetMatchesFlipX ? -1 : 1; // set the sign of the X position based on whether the player 
														// sprite is flipped or not
				anim.transform.localPosition = new Vector2(anim.transform.localPosition.x * sign, anim.transform.localPosition.y);
			}
			// Set the rotation for the effect
			Quaternion rot = GetRotation();

			// Initialize the effect properties
			TempObjectInfo info = new TempObjectInfo();
			info.targetColor = color;
			if (duration < 0)
				duration = anim.anim.TimeLength;
			info.lifeTime = duration;
			info.fadeOutTime = 0.1f;

			// Initialize the effect and play it
			anim.GetComponent<TempObject>().Init(
				rot,
				anim.transform.position,
				anim.anim.frames[0],
				info);
			anim.Play();
		}

		private Quaternion GetRotation()
		{
			float angle;
			switch (rotationType) 
			{
				case RotationType.Same:
					angle = Mathf.Atan2(player.dir.y, player.dir.x) * Mathf.Rad2Deg;
					break;
				case RotationType.Opposite:
					angle = Mathf.Atan2(-player.dir.y, -player.dir.x) * Mathf.Rad2Deg;
					break;
				case RotationType.MatchFlipX:
					angle = player.sr.flipX ? 180 : 0;
					break;
				case RotationType.ReverseFlipX:
					angle = player.sr.flipX ? 0 : 180;
					break;
				default:
					angle = 0;
					break;
			}
			return Quaternion.Euler(new Vector3(0, 0, angle));
		}
	}
}
