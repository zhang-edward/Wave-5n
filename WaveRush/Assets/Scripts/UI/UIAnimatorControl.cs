using UnityEngine;
using System.Collections;

public class UIAnimatorControl : MonoBehaviour
{
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	public void AnimateOut()
	{
		StartCoroutine(AnimateOutRoutine());
	}

	private IEnumerator AnimateOutRoutine()
	{
		anim.CrossFade("Out", 0);
		yield return new WaitForEndOfFrame();
		while (anim.GetCurrentAnimatorStateInfo(0).IsName("Out"))
			yield return null;
		gameObject.SetActive(false);
	}
}