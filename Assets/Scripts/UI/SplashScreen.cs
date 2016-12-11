using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class SplashScreen : MonoBehaviour {

	public Image overlay;

	void Awake()
	{
		StartCoroutine (DoSplashScreen ());
	}

	void Update()
	{
		if (Input.anyKeyDown) 
		{
			SceneManager.LoadScene ("StartScreen");
		}
	}

	private IEnumerator DoSplashScreen()
	{
		overlay.color = Color.black;
		float t = 0;
		// fade in
		while (t < 1)
		{
			overlay.color = Color.Lerp (Color.black, Color.clear, t);
			t += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds (2.0f);
		// fade out
		while (t > 0)
		{
			overlay.color = Color.Lerp (Color.black, Color.clear, t);
			t -= Time.deltaTime;
			yield return null;
		}
		SceneManager.LoadScene ("StartScreen");
	}
}
