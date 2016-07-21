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
		text.text = "Velocity: " + player.body.Rb2d.velocity.magnitude;
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
