using UnityEngine;
using System.Collections;
/*
public class ShopNPC : MonoBehaviour
{
	private bool activated;		// whether the player can collide with this shopkeeper to see the shop UI
	private SpriteRenderer sr;
	
	public PowerUpMenu shop;
	public SimpleAnimationPlayer smokeAnim;
	public SimpleAnimationPlayer npcAnim;
	public SimpleAnimationPlayer dialogueAnim;

	[Header("Audio")]
	public AudioClip smokePoof;
	public AudioClip shopNotification;

	void Awake()
	{
		sr = GetComponent<SpriteRenderer> ();
	}

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
		yield return null;	// wait 1 frame for the gameObject.SetActive(true) to be registered

		SoundManager.instance.PlaySingle (smokePoof);
		sr.color = Color.clear;
		smokeAnim.Play ();
		// wait 3 frames for the smoke animation to play
		float waitTime = smokeAnim.anim.SecondsPerFrame * 3;
		yield return new WaitForSeconds (waitTime);
		sr.color = Color.white;

		npcAnim.Play ();
		dialogueAnim.Play ();

		yield return new WaitForSeconds (1.0f);
		SoundManager.instance.PlaySingle (shopNotification);
		activated = true;
	}

	private IEnumerator DisappearRoutine()
	{
		activated = false;
		yield return new WaitForSeconds (1.0f);
		SoundManager.instance.PlaySingle (smokePoof);
		sr.color = Color.clear;
		smokeAnim.Play ();
		while (smokeAnim.isPlaying)
			yield return null;
		gameObject.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (activated && col.CompareTag("Player"))
		{
			shop.GetShopItems ();
			shop.AnimateIn ();
		}
	}
}
*/