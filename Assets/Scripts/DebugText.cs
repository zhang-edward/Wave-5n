using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugText : MonoBehaviour {

	private Text text;
	public Player player;

	int count;

	void Awake()
	{
		text = GetComponent<Text> ();
	}

	void Update()
	{
		text.text = "Damage taken: " + count;
	}

	void OnEnable()
	{
		player.OnPlayerDamaged += Counter;
	}

	void OnDisable()
	{
		player.OnPlayerDamaged -= Counter;
	}

	public void Counter(int amt)
	{
		count += amt;
	}
}
