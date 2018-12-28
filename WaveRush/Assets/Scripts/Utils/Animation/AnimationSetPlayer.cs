using UnityEngine;
using System.Collections;

public class AnimationSetPlayer : SimpleAnimationPlayer
{
	public AnimationSetAnim defaultAnim;
	public bool willResetToDefault = true;

	void Update()
	{
		if (!isPlaying && willResetToDefault)
		{
			looping = true;
			Play(defaultAnim);
		}
	}

	public void ResetToDefault()
	{
		willResetToDefault = true;
		looping = true;
		Play(defaultAnim);
	}

	/// <summary>
	/// Checks if an animation is currently being played
	/// </summary>
	/// <param name="animationName">Animation to check for.</param>
	public bool IsPlayingAnimation(string animationName) {
		return anim.animationName.Equals(animationName);
	}
}
