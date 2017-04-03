using UnityEngine;
public class DataManager : MonoBehaviour
{
	public static DataManager instance;

	public HeroPowerUpListData[] powerUpList;   // power up info for every hero
	public HeroDescriptionData[] heroDescriptions;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);
		DontDestroyOnLoad(this);
	}

	public static HeroPowerUpListData GetPowerUpListData(HeroType type)
	{
		foreach (HeroPowerUpListData data in instance.powerUpList)
		{
			if (data.type == type)
				return data;
		}
		throw new UnityEngine.Assertions.AssertionException
		                     (instance.GetType().Name,
							  "Could not find data for hero with name " + type.ToString() + "!");
	}

	public static HeroDescriptionData GetDescriptionData(HeroType type)
	{
		foreach (HeroDescriptionData data in instance.heroDescriptions)
		{
			if (data.type == type)
				return data;
		}
		throw new UnityEngine.Assertions.AssertionException
							 (instance.GetType().Name,
							  "Could not find data for hero with name " + type.ToString() + "!");
	}
}
