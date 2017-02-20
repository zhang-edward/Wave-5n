using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {

	public SimpleAnimationPlayer animPlayer;

	void OnEnable()
	{
		StartCoroutine (Shine());
	}

	private IEnumerator Shine()
	{
		while (true)
		{
			animPlayer.Play ();
			yield return new WaitForSeconds (3.0f);
		}
	}
}
