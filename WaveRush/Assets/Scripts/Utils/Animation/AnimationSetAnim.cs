using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationSetAnim : SimpleAnimation
{
	[Tooltip ("Whether this animation will loop on completion")] 
	public bool looping = false;
	[Tooltip ("Whether this animation will reset to default upon completion, or stick to the last frame")]
	public bool resetToDefault = true;
	[Tooltip ("Whether this animation will always play in realtime")]
	public bool ignoreTimeScale = false;

	public void SetTimeLength(float timeLength)
	{
		fps = 1 / (timeLength / frames.Length);
	}
}
