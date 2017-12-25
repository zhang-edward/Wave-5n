using UnityEngine;
using System.Collections;

public class AnimationSetPlayer : SimpleAnimationPlayer
{
	public AnimationSetAnim defaultAnim;
	public bool willResetToDefault = true;

	public void Init()
	{
		StartCoroutine(ResetToDefaultAnimation());	
	}

	private IEnumerator ResetToDefaultAnimation()
	{
		for (;;)
		{
			if (!isPlaying && willResetToDefault)
			{
				anim = defaultAnim;
				looping = true;
				Play();
			}
			yield return null;
		}
	}

	public void ResetToDefault()
	{
		willResetToDefault = true;
		anim = defaultAnim;
		looping = true;
		Play();
	}
}
