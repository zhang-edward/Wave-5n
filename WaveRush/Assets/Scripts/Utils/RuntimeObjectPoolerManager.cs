using System.Collections.Generic;
using UnityEngine;

public class RuntimeObjectPoolerManager : MonoBehaviour {

	public static RuntimeObjectPoolerManager instance;
	public GameObject runtimeObjectPoolerPrefab;

	public List<RuntimeObjectPooler> objectPoolers = new List<RuntimeObjectPooler>();

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);
	}

	public RuntimeObjectPooler CreateRuntimeObjectPooler(string poolName, GameObject prefab)
	{
		GameObject o = Instantiate(runtimeObjectPoolerPrefab);
		o.transform.SetParent(transform);

		RuntimeObjectPooler pooler = o.GetComponent<RuntimeObjectPooler>();
		pooler.poolType = poolName;
		pooler.pooledObject = prefab;
		return pooler;
	}
}
