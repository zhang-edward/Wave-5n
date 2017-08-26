using UnityEngine;
using System.Collections;

public class IndicatorEffect : MonoBehaviour
{
	public SimpleAnimation initAnimation;
	public SimpleAnimation loopAnimation;
	public SimpleAnimation onDisableAnimation;

	private SimpleAnimationPlayer anim;
	private bool animatingOut;

	void Awake()
	{
		anim = GetComponent<SimpleAnimationPlayer> ();
	}

	void OnEnable()
	{
		StartCoroutine ("DoAnimation");
	}

	private IEnumerator DoAnimation()
	{
		anim.anim = initAnimation;
		anim.looping = false;
		anim.Play ();

		while (anim.isPlaying)
			yield return null;

		anim.anim = loopAnimation;
		anim.looping = true;
		anim.Play ();

		while (!animatingOut)
			yield return null;

		anim.anim = onDisableAnimation;
		anim.looping = false;
		anim.Play ();

		while (anim.isPlaying)
			yield return null;

		animatingOut = false;
		gameObject.SetActive (false);
	}

	public void AnimateOut()
	{
		if (!gameObject.activeInHierarchy)
			return;
		animatingOut = true;
	}

	public void SetAnimatingOut(bool b)
	{
		animatingOut = b;
	}
}

