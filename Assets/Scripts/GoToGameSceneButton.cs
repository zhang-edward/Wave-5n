using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoToGameSceneButton : MonoBehaviour {

	private Button button;

	void Awake()
	{
		button = GetComponent<Button> ();
	}

	void Start()
	{
		button.onClick.AddListener(() => {GameManager.instance.GoToGameScene();});
	}
}
