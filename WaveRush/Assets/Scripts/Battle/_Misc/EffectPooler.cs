using UnityEngine;
using System.Collections;

public class EffectPooler : ObjectPooler
{
	private readonly Quaternion DEFAULT_ROT = Quaternion.identity;

	public static EffectPooler instance;
	public const string EFFECT_POOL_NAME = "Effect";

	public override void Init()
	{
		poolType = EFFECT_POOL_NAME;
		base.Init();
		instance = this;
	}

	public static TempObject PlayEffect(SimpleAnimation toPlay, Vector3 position, bool randRotation = false, float fadeOutTime = 0f)
	{
		GameObject o = instance.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, toPlay.TimeLength - fadeOutTime, fadeOutTime, new Color(1, 1, 1, 1));
		Quaternion rot;
		if (randRotation)
			rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
		else
			rot = Quaternion.identity;
		tempObj.Init(rot,
					 position,
					 toPlay.frames[0]);
		anim.Play(toPlay);
		return tempObj;
	}

	public static TempObject PlayEffect(SimpleAnimation toPlay, Vector3 position, TempObjectInfo info)
	{
		GameObject o = instance.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = info;
		tempObj.Init(Quaternion.identity,
					 position,
					 toPlay.frames[0]);
		anim.Play(toPlay);
		return tempObj;
	}

	public static TempObject PlayEffect(SimpleAnimation toPlay, Vector3 position, TempObjectInfo info, Quaternion rot)
	{
		GameObject o = instance.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = info;
		tempObj.Init(rot,
					 position,
					 toPlay.frames[0]);
		anim.Play(toPlay);
		return tempObj;
	}
}
