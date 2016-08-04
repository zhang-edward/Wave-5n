using UnityEngine;
using System.Collections;

public class RuntimeObjectPooler : ObjectPooler
{
	void Awake()
	{
		
	}

	public void SetPooledObject(GameObject newObj)
	{
		pooledObject = newObj;
		pooledObjects = new System.Collections.Generic.List<GameObject> ();
		for (int i = 0; i < poolAmount; i ++)
		{
			GameObject obj = Instantiate (pooledObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			obj.transform.SetParent (this.transform);
			obj.SetActive (false);
			pooledObjects.Add (obj);
		}
	}
}

