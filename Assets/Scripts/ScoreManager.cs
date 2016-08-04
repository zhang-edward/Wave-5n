using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

	/// <summary>
	/// The high scores, each hero gets his own high score
	/// </summary>
	public Dictionary<string, int> highScores;

	/// <summary>
	/// Submits the score.
	/// </summary>
	/// <returns><c>true</c>, if score was a high score, <c>false</c> otherwise.</returns>
	/// <param name="hero">Hero.</param>
	/// <param name="score">Score.</param>
	public bool SubmitScore(string hero, int score)
	{
		if (highScores [hero] < score)
		{
			highScores [hero] = score;
			return true;
		}
		else
			return false;
	}
}
