using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

	[System.Serializable]
	public class Score {
		public int enemiesDefeated;
		public int wavesSurvived;
		public int maxCombo;

		public Score(int enemiesDefeated, int wavesSurvived, int maxCombo)
		{
			this.enemiesDefeated = enemiesDefeated;
			this.wavesSurvived = wavesSurvived;
			this.maxCombo = maxCombo;
		}

		public void UpdateScore(Score other)
		{
			if (other.enemiesDefeated > enemiesDefeated)
				this.enemiesDefeated = other.enemiesDefeated;
			if (other.wavesSurvived > wavesSurvived)
				this.wavesSurvived = other.wavesSurvived;
			if (other.maxCombo > maxCombo)
				this.maxCombo = other.maxCombo;
		}

		public override string ToString ()
		{
			return "Enemies Killed: " + enemiesDefeated +
			"\nWaves Survived" + wavesSurvived;
		}
	}

	public Dictionary<HeroType, Score> highScores = new Dictionary<HeroType, Score>();

	/// <summary>
	/// Submits the score.
	/// </summary>
	/// <param name="hero">Hero.</param>
	/// <param name="score">Score.</param>
	public void SubmitScore(HeroType hero, Score score)
	{
		if (highScores.ContainsKey (hero))
			highScores [hero].UpdateScore (score);
		else
			highScores.Add (hero, score);
	}
}
