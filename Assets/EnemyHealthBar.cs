using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour {

	public Slider slider;
	public Image fillArea;
	public Enemy enemy;
	private RectTransform rect;

	void Awake()
	{
		rect = GetComponent<RectTransform> ();
	}

	void Init(Enemy enemy)
	{
		this.enemy = enemy;
		slider.maxValue = enemy.maxHealth;
	}

	void Update()
	{
		slider.value = enemy.health;
		rect.anchoredPosition = enemy.transform.position + new Vector3(0, 1);
	}
}
