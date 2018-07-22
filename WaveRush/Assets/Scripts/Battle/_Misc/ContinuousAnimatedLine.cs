using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousAnimatedLine : MonoBehaviour {

	public GameObject boltPrefab;
	public GameObject boltHead;
	public float segmentWidth;		// the width of one lightning bolt segment
	private Vector3 start, end;

	public void Init(Vector3 start, Vector3 end)
	{
		this.start = start;
		this.end = end;

		CreateBolt ();
	}

	public void CreateBolt()
	{
		GameObject startHead = Instantiate (boltHead, start, Quaternion.identity) as GameObject;
		GameObject endHead = Instantiate (boltHead, end, Quaternion.identity) as GameObject;
		startHead.transform.SetParent (this.transform);
		endHead.transform.SetParent (this.transform);

		Vector3 normalizedVector = (start - end).normalized;
		float distance = Vector2.Distance (start, end);
		int numSegments = Mathf.RoundToInt(distance / segmentWidth);

		float angle = Mathf.Atan2 (normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
		Quaternion rot = Quaternion.Euler (new Vector3 (0, 0, angle));

		for (int i = 0; i < numSegments; i ++)
		{
			GameObject o = Instantiate (boltPrefab);
			Vector2 pos = start + (normalizedVector * -i * segmentWidth) - (normalizedVector * segmentWidth / 2f);
			o.transform.position = pos;
			o.transform.rotation = rot;
			o.transform.parent = this.transform;

			o.GetComponent<SpriteRenderer> ().flipX = Random.value < 0.5f;
		}
	}
}
