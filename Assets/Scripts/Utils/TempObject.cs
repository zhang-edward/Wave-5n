using UnityEngine;
using System.Collections;

/// <summary>
/// A temporary object which disappears after some time, such as an effect
/// </summary>
public class TempObject : MonoBehaviour {

	public float targetAlpha = 1;
	public bool isSelfDeactivating = false;
	public float fadeInTime = 0;
	public float lifeTime = 0;
	public float fadeOutTime = 0.1f;

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
	public void Init(Quaternion rotation, Vector3 position, Sprite sprite, bool isSelfDeactivating, float fadeInTime = 0, float lifeTime = 0, float fadeOutTime = 0.2f)
	{
		gameObject.SetActive (true);
		transform.rotation = rotation;
		transform.position = position;
		sr.sprite = sprite;
		this.isSelfDeactivating = isSelfDeactivating;
		this.fadeInTime = fadeInTime;
		this.lifeTime = lifeTime;
		this.fadeOutTime = fadeOutTime;
		StartCoroutine (FadeIn());
	}

	/// <summary>
	/// Init the specified rotation, position and sprite, with <see cref="isSelfDeactivating/>, <see cref="fadeInTime/>,
	/// <see cref="lifetime"/>, and <see cref="fadeOutTime/> set to their default values set in the Inspector
	/// </summary>
	/// <param name="rotation">Rotation.</param>
	/// <param name="position">Position.</param>
	/// <param name="sprite">Sprite.</param>
	public void Init(Quaternion rotation, Vector3 position, Sprite sprite)
	{
		gameObject.SetActive (true);
		transform.rotation = rotation;
		transform.position = position;
		sr.sprite = sprite;
		StartCoroutine (FadeIn());
	}

	private IEnumerator FadeIn()
	{
		sr.color = new Color (1, 1, 1, 0);
		float t = 0;
		while (t < fadeInTime)
		{
			t += Time.deltaTime;
			sr.color = Color.Lerp(new Color(1, 1, 1, 0), 
				new Color(1, 1, 1, targetAlpha),
				t / fadeInTime);
			yield return null;
		}
		sr.color = new Color (1, 1, 1, targetAlpha);
		if (isSelfDeactivating)
		{
			yield return new WaitForSeconds (lifeTime);
			StartCoroutine (FadeOut ());
		}
	}

	private IEnumerator FadeOut()
	{
		Color initialColor = sr.color;
		float t = 0;
		while (t < fadeOutTime)
		{
			t += Time.deltaTime;
			sr.color = Color.Lerp(initialColor, 
				new Color(1, 1, 1, 0),
				t / fadeOutTime);
			yield return null;
		}
		gameObject.SetActive (false);
	}

	public void Deactivate()
	{
		StartCoroutine (FadeOut ());
	}
}
