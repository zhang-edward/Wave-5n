using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

	[System.Serializable]
	public class Score {
		public int enemiesDefeated;
		public int wavesSurvived;

		public Score(int enemiesDefeated, int wavesSurvived)
		{
			this.enemiesDefeated = enemiesDefeated;
			this.wavesSurvived = wavesSurvived;
		}

		public void UpdateScore(Score other)
		{
			if (other.enemiesDefeated > enemiesDefeated)
				this.enemiesDefeated = other.enemiesDefeated;
			if (other.wavesSurvived > wavesSurvived)
				this.wavesSurvived = other.wavesSurvived;
		}

		public override string ToString ()
		{
			return "Enemies Killed: " + enemiesDefeated +
			"\nWaves Survived" + wavesSurvived;
		}
	}

	public Dictionary<string, Score> highScores = new Dictionary<string, Score>();

	/// <summary>
	/// Submits the score.
	/// </summary>
	/// <param name="hero">Hero.</param>
	/// <param name="score">Score.</param>
	public void SubmitScore(string hero, Score score)
	{
		if (highScores.ContainsKey (hero))
			highScores [hero].UpdateScore (score);
		else
			highScores.Add (hero, score);
	}
}
