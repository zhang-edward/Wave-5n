using UnityEngine;
using System.Collections;

public class BossSpawn : MonoBehaviour {

	public SimpleAnimationPlayer[] extras;

	public void PlayAnimation()
	{
		this.GetComponent<SimpleAnimationPlayer> ().Play ();
		foreach (SimpleAnimationPlayer sap in extras)
			sap.Play ();
	}
}
