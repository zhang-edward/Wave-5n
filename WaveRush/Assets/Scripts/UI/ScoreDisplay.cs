using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public Text enemiesDefeated;
	public Text wavesSurvived;

	public void DisplayScores(HeroType type)
	{
		string hero = type.ToString();
		ScoreManager sm = GameManager.instance.scoreManager;
		if (sm.highScores.ContainsKey(hero))
		{
			ScoreManager.Score score = sm.highScores [hero];
			enemiesDefeated.text = "Most Enemies Defeated: " + score.enemiesDefeated;
			wavesSurvived.text = "Most Waves Survived: " + score.wavesSurvived;	
		}
		else
		{
			enemiesDefeated.text = "Most Enemies Defeated: 0";
			wavesSurvived.text = "Most Waves Survived: 0";	
		}
	}
}
