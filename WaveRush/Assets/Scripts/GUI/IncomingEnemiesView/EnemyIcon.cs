using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyIcon : MonoBehaviour {

	public Image image;
	public TMP_Text countText;

	public void Init(Enemy enemy, int count) {
		image.sprite = enemy.anim.GetAnimation("Default").frames[0];
		countText.text = "x" + count.ToString();
	}
}