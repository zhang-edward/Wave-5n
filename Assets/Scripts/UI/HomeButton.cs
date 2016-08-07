using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HomeButton : MonoBehaviour {

	private Button button;

	void Awake()
	{
		button = GetComponent<Button> ();
	}

	void Start()
	{
		button.onClick.AddListener(() => {GameManager.instance.GoToMenuScene();});
	}
}
