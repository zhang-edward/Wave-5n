using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour {

	public Slider slider;
	public Image fillArea;

	public Color fullColor; 	// color of health bar when health is full
	public Color damagedColor;	// color of health bar when health is below max health
	public Color criticalColor; // color of health bar when player can kill this enemy in one hit

	public bool movesWithEnemy = true;
	private Enemy enemy;
	public Player player;

	private RectTransform rect;

	void Awake()
	{
		rect = GetComponent<RectTransform> ();
	}
		
	public void Init(Enemy enemy)
	{
		this.enemy = enemy;
		slider.maxValue = enemy.maxHealth;
		gameObject.SetActive (true);
	}

	void Update()
	{
		slider.value = enemy.health;
		SetFillAreaColor ();
		if (movesWithEnemy)
			rect.anchoredPosition = enemy.transform.position + new Vector3(0, 0.5f);
		if (!enemy.gameObject.activeInHierarchy)
			gameObject.SetActive (false);
	}

	private void SetFillAreaColor()
	{
		if (enemy.health <= player.hero.damage)
		{
			fillArea.color = criticalColor;
		}
		else
		{
			fillArea.color = Color.Lerp (damagedColor, fullColor, (float)enemy.health / enemy.maxHealth);
		}
	}
}
