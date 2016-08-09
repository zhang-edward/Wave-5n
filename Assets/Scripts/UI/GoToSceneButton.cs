using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoToSceneButton : MonoBehaviour {

	public string sceneName;
	private Button button;

	void Awake()
	{
		button = GetComponent<Button> ();
	}

	void Start()
	{
		button.onClick.AddListener(() => {GameManager.instance.GoToScene(sceneName);});
	}
}
