using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationSetAnim : SimpleAnimation
{
	public bool looping;
	public bool ignoreTimeScale;

	public void SetTimeLength(float timeLength)
	{
		fps = 1 / (timeLength / frames.Length);
	}
}
