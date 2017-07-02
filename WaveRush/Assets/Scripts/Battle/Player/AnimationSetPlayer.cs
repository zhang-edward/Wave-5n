using UnityEngine;
using System.Collections;

public class AnimationSetPlayer : SimpleAnimationPlayer
{
	public AnimationSetAnim defaultAnim;

	public void Init()
	{
		StartCoroutine(ResetToDefaultAnimation());	
	}

	private IEnumerator ResetToDefaultAnimation()
	{
		for (;;)
		{
			if (!isPlaying)
			{
				anim = defaultAnim;
				looping = true;
				Play();
			}
			yield return null;
		}
	}
}
