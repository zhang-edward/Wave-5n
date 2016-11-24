using UnityEngine;
using System.Collections;

public class ShopNPC : MonoBehaviour
{
	public Shop shop;
	public SimpleAnimationPlayer smokeAnim;

	public void Appear()
	{
		gameObject.SetActive (true);
		StartCoroutine ("AppearRoutine");
	}

	public void Disappear()
	{
		StartCoroutine ("DisappearRoutine");
	}

	private IEnumerator AppearRoutine()
	{
		yield return null;
		smokeAnim.Play ();
	}

	private IEnumerator DisappearRoutine()
	{
		yield return new WaitForSeconds (1.0f);
		smokeAnim.Play ();
		while (smokeAnim.isPlaying)
			yield return null;
		gameObject.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			shop.AnimateIn ();
		}
	}
}
