using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	public HeroSaveData[] heroData;
	[Serializable]
	public class HeroSaveData 
	{
		public HeroType hero;
		public bool unlocked;
	}

	public List<HeroConsumable> heroConsumables = new List<HeroConsumable>();

	public Dictionary<string, ScoreManager.Score> highScores;
	public Wallet wallet;

	public SaveGame()
	{
		int numHeroTypes = Enum.GetNames(typeof(HeroType)).Length;
		// Debug.Log(numHeroTypes);
		// default all heroes locked but the first hero (the knight)
		heroData = new HeroSaveData[numHeroTypes];
		for (int i = 0; i < numHeroTypes; i ++)
		{
			HeroType type = (HeroType)Enum.GetValues(typeof(HeroType)).GetValue(i);
			heroData[i] = new HeroSaveData();
			heroData[i].hero = type;
		}
		heroData[0].unlocked = true;
		// high scores are all 0 by default
		highScores = new Dictionary<string, ScoreManager.Score>();
		// wallet money = 0 by default
		wallet = new Wallet();
	}

	public void ClearHighScores()
	{
		highScores = new Dictionary<string, ScoreManager.Score>();
	}

	public HeroSaveData GetHeroData(HeroType hero)
	{
		foreach (HeroSaveData data in heroData)
		{
			if (data.hero == hero)
				return data;
		}
		return null;
	}
}