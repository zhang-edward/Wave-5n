using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {

	private bool isSelfDeactivating = false;

	private SpriteRenderer sr;
	void Awake () {
		sr = GetComponent<SpriteRenderer> ();
	}

	/// <summary>
	/// Init the specified rotation, sprite and isSelfDeactivating variables.
	/// </summary>
	/// <param name="rotation">Rotation.</param>
	/// <param name="sprite">Sprite.</param>
	/// <param name="isSelfDeactivating">If set to <c>true</c> is self deactivating.</param>
	/// <param name="fadeInTime">Amount of time for this effect to fade in</param>
	public void Init(Quaternion rotation, Vector3 position, Sprite sprite, bool isSelfDeactivating, float fadeInTime = 0)
	{
		gameObject.SetActive (true);
		transform.rotation = rotation;
		transform.position = position;
		sr.sprite = sprite;
		this.isSelfDeactivating = isSelfDeactivating;
		StartCoroutine (FadeIn (fadeInTime));
	}

	private IEnumerator FadeIn(float fadeInTime)
	{
		sr.color = new Color (1, 1, 1, 0);
		float t = 0;
		while (t < fadeInTime)
		{
			t += Time.deltaTime;
			sr.color = Color.Lerp(new Color(1, 1, 1, 0), 
				new Color(1, 1, 1, 1),
				t / fadeInTime);
			yield return null;
		}
		sr.color = new Color (1, 1, 1, 1);
		if (isSelfDeactivating)
			StartCoroutine (FadeOut ());
	}

	private IEnumerator FadeOut()
	{
		while (sr.color.a > 0.1f)
		{
			sr.color = new Color (1, 1, 1, sr.color.a - 0.1f);
			yield return null;
		}
		gameObject.SetActive (false);
	}

	public void Deactivate()
	{
		StartCoroutine (FadeOut ());
	}
}
