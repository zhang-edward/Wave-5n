using UnityEngine;
using System.Collections;

public class PlayerInfoHolder : MonoBehaviour
{
	[System.Serializable]
	public class PlayerInfo
	{
		public GameObject prefab;
		public string name;
		public PlayerHero hero { get; private set; }
	}

	public PlayerInfo[] infos;

	public PlayerInfo GetPlayerInfo(string name)
	{
		foreach(PlayerInfo info in infos)
		{
			if (info.name == name)
				return info;
		}
		Debug.LogError ("Couldn't find PlayerInfo with name " + name);
		return null;
	}

	public PlayerHero InitHero(string name)
	{
		PlayerInfo info = GetPlayerInfo (name);
		GameObject o = Instantiate (info.prefab, transform.position, Quaternion.identity) as GameObject;
		o.transform.SetParent (this.transform);
		return o.GetComponent<PlayerHero> ();
	}
}

