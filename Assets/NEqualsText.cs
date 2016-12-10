using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NEqualsText : MonoBehaviour {

	public Sprite[] numberSprites;
	public GameObject numberPrefab;
	public Image nEquals;
	private List<GameObject> digits = new List<GameObject>();

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A))
		{
			int num = Random.Range (0, 100);
			Debug.Log (num);
			Display (num);
		}
	}

	public void Display(int number)
	{
		// clear any previous digits
		foreach (GameObject o in digits)
			Destroy (o);
		while (number > 0)
		{
			int digit = number % 10;

			GameObject o = Instantiate (numberPrefab);
			o.transform.SetParent (this.transform, false);
			o.GetComponent<Image> ().sprite = numberSprites [digit];
			o.transform.SetSiblingIndex (1);	// set the digit to be to the left of the previous one
			digits.Add(o);

			number /= 10;
		}
		CameraControl.instance.StartShake (0.1f, 0.1f);
		nEquals.CrossFadeAlpha (0, 1f, false);
	}
}
