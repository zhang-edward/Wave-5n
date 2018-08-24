using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimationSet
{
	public string animationSetName;
	public List<AnimationSetAnim> animations;
	private Dictionary<string, AnimationSetAnim> animDictionary;
	[HideInInspector]
	public AnimationSetPlayer player;

	public void Init(AnimationSetPlayer player)
	{
		this.player = player;
		animDictionary = new Dictionary<string, AnimationSetAnim>();
		foreach(AnimationSetAnim anim in animations)
			animDictionary.Add(anim.animationName, anim);
		player.defaultAnim = animDictionary["Default"];
	}

	public bool Play(string name) {
		if (!animDictionary.ContainsKey(name))
			return false;
		AnimationSetAnim animation = animDictionary[name];
		player.looping = animation.looping;
		player.willResetToDefault = animation.resetToDefault;
		player.ignoreTimeScaling = animation.ignoreTimeScale;
		player.Play(animation);
		return true;
	}

	public void Play(AnimationSetAnim animation) {
		player.looping = animation.looping;
		player.willResetToDefault = animation.resetToDefault;
		player.ignoreTimeScaling = animation.ignoreTimeScale;
		player.Play(animation);
	}

	public AnimationSetAnim GetAnimation(string name) {
		if (animDictionary != null)
			return animDictionary[name];
		else {
			foreach (AnimationSetAnim anim in animations) {
				if (anim.animationName == name)
					return anim;
			}
			return null;
		}
	}
}
