using UnityEngine;
using System.Collections;

public class StatusEffectContainer : MonoBehaviour
{
	public static StatusEffectContainer instance;
	public SimpleAnimation confusedEffect;

	void Awake()
	{
		// make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
	}
}

