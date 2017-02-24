using UnityEngine;
using System.Collections;

public class KnightRushPowerUp : HeroPowerUp
{
	private const float MULTIPLIER = 1.15f;

	private KnightHero knight;
	private float totalSpeedMultiplier;		// the amount of speed that this powerup adds to the rush effect

	[Header("Animations")]
	public TempObject[] effects;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.knight = (KnightHero)hero;
		knight.rushMoveSpeedMultiplier *= MULTIPLIER;
		totalSpeedMultiplier = MULTIPLIER;
		knight.OnKnightRush += PlayEffect;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		knight.rushMoveSpeedMultiplier /= totalSpeedMultiplier;
	}

	public override void Stack ()
	{
		base.Stack ();
		knight.rushMoveSpeedMultiplier *= MULTIPLIER;
		totalSpeedMultiplier *= MULTIPLIER;
	}

	private void PlayEffect()
	{
		for (int i = 0; i < stacks + 1; i ++)
		{
			SimpleAnimationPlayer animPlayer = effects[i].GetComponent<SimpleAnimationPlayer> ();

			Vector2 dir = knight.player.dir;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			TempObjectInfo info = new TempObjectInfo ();
			info.isSelfDeactivating = true;
			info.lifeTime = animPlayer.anim.TimeLength;
			info.targetColor = new Color (1, 1, 1, 1f);
			effects[i].Init (
				Quaternion.Euler (new Vector3 (0, 0, angle)),
				transform.position,
				animPlayer.anim.frames[0],
				info
			);
			animPlayer.Play ();
			print (effects [i] + " is playing");
		}
	}
}

