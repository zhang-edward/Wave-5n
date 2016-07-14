using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

	public static List<ObjectPooler> objectPoolers = new List<ObjectPooler>();

	public string poolType;
	public GameObject pooledObject;
	public int poolAmount = 10;
	public bool willGrow = true;

	private List<GameObject> pooledObjects; 

	void Awake()
	{
		foreach (ObjectPooler pooler in objectPoolers)
		{
			if (pooler.poolType == this.poolType)
			{
				Debug.LogError ("Duplicate objectPooler detected");
				Destroy (this.gameObject);
			}
		}
		objectPoolers.Add (this);
	}

	void Start()
	{
		pooledObjects = new List<GameObject> ();
		for (int i = 0; i < poolAmount; i ++)
		{
			GameObject obj = Instantiate (pooledObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			obj.transform.SetParent (this.transform);
			obj.SetActive (false);
			pooledObjects.Add (obj);
		}
	}

	public GameObject GetPooledObject()
	{
		foreach (GameObject obj in pooledObjects)
		{
			if (!obj.activeInHierarchy)
				return obj;
		}

		if (willGrow)
		{
			GameObject obj = Instantiate (pooledObject);
			obj.transform.SetParent (this.transform);
			pooledObjects.Add (obj);
			return obj;
		}
		return null;
	}

	public static ObjectPooler GetObjectPooler(string name)
	{
		foreach (ObjectPooler pooler in objectPoolers)
		{
			if (pooler.poolType == name)
			{
				return pooler;
			}
		}
		return null;
	}
}
