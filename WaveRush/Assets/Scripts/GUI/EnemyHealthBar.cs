using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour {

	[Header("Slider")]
	public Slider slider;
	public Image fillArea;

	public Color fullColor; 	// color of health bar when health is full
	public Color damagedColor;	// color of health bar when health is below max health
	public Color criticalColor; // color of health bar when player can kill this enemy in one hit

	public bool fixedPos;
	private Enemy enemy;
	public Player player;	// used for setting the critical health bar color (See above)


	[Header("Statuses")]
	public GameObject abilityIconBar;
	public GameObject imagePrefab;
	public List<GameObject> icons = new List<GameObject>();

	void Awake()
	{
		for (int i = 0; i < 4; i ++)
		{
			GameObject o = Instantiate (imagePrefab);
			o.transform.SetParent (abilityIconBar.transform, false);
			icons.Add (o);
		}
	}

	/// <summary>
	/// Init the health bar with the specified enemy.
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	public void Init(Enemy enemy)
	{
		gameObject.SetActive (true);
		abilityIconBar.SetActive (true);

		this.enemy = enemy;
		slider.maxValue = enemy.maxHealth;
		if (!fixedPos)
		{
			slider.transform.localPosition = enemy.healthBarOffset;
		}
		foreach (GameObject o in icons)
			o.SetActive (false);
		for (int i = 0; i < enemy.abilities.Count; i ++)
		{
			EnemyAbility ability = enemy.abilities [i];
			if (ability.icon != null)
			{
				icons [i].GetComponent<Image> ().sprite = ability.icon;
				icons [i].SetActive (true);
			}
		}
	}

	void Update()
	{
		slider.value = enemy.health;
		SetFillAreaColor ();
		if (enemy == null || !enemy.gameObject.activeInHierarchy)
		{
			gameObject.SetActive (false);
			abilityIconBar.SetActive (false);
		}
	}

	private void SetFillAreaColor()
	{
		if (enemy.health <= player.hero.noiselessDamage)
		{
			fillArea.color = criticalColor;
		}
		// set to smooth transition between full and damaged color
		else
		{
			fillArea.color = Color.Lerp (damagedColor, fullColor, (float)enemy.health / enemy.maxHealth);
		}
	}
}
