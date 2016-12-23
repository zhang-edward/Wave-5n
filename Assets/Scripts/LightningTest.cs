using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTest : MonoBehaviour {

	public Vector3 start;
	public Vector3 end;
	public LightningBolt bolt;

	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		if (Input.GetMouseButtonDown(1))
		{
			end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			bolt.Init (start, end);
			start = end;
		}
	}
}
