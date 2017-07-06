using UnityEngine;
using System.Collections;

public class FireTrailEmitter : MonoBehaviour
{
	public GameObject fireTrailPrefab;
	public int damage;

	void Start()
	{
		InvokeRepeating ("CreateTrail", 0f, 0.5f);
	}

	void OnDisable()
	{
		CancelInvoke ();
	}

	private void CreateTrail()
	{
		GameObject o = Instantiate (fireTrailPrefab, transform.position, Quaternion.identity) as GameObject;
		o.transform.SetParent (ObjectPooler.GetObjectPooler ("Effect").transform);
		o.GetComponent<MageFire>().damage = damage;
	}
}

