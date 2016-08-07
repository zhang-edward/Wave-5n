using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour {

	public Slider slider;
	public Image fillArea;

	public Color fullColor; 	// color of health bar when health is full
	public Color damagedColor;	// color of health bar when health is below max health
	public Color criticalColor; // color of health bar when player can kill this enemy in one hit

	public bool movesWithEnemy = true;		// set to false for boss health bars
	private Enemy enemy;
	public Player player;	// used for setting the critical health bar color (See above)

	private RectTransform rect;

	void Awake()
	{
		rect = GetComponent<RectTransform> ();
	}

	/// <summary>
	/// Init the health bar with the specified enemy.
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	public void Init(Enemy enemy)
	{
		this.enemy = enemy;
		slider.maxValue = enemy.maxHealth;
		if (movesWithEnemy)
			transform.position = enemy.transform.position;
		gameObject.SetActive (true);
	}

	void Update()
	{
		slider.value = enemy.health;
		SetFillAreaColor ();
		if (movesWithEnemy)
			rect.anchoredPosition = enemy.transform.position + enemy.healthBarOffset;
		if (!enemy.gameObject.activeInHierarchy)
			gameObject.SetActive (false);
	}

	private void SetFillAreaColor()
	{
		if (enemy.health <= player.hero.damage)
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
