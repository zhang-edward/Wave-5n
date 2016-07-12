using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour {

	private SpriteRenderer sr;
	void Awake () {
		sr = GetComponent<SpriteRenderer> ();
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360)));
	}

	void Update()
	{
		sr.color = new Color (1, 1, 1, sr.color.a - 0.1f);
		if (sr.color.a < 0.1)
			gameObject.SetActive (false);
	}
}
