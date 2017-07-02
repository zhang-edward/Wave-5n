using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimationSet
{
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

	public void Play(string name)
	{
		AnimationSetAnim animation = animDictionary[name];
		player.anim = animation;
		player.looping = animation.looping;
		player.Play();
	}
}
