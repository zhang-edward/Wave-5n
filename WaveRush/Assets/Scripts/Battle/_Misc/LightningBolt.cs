using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour {

	public GameObject boltPrefab;
	public GameObject boltHead;
	public float segmentWidth;		// the width of one lightning bolt segment
	private Vector2 start, end;

	public void Init(Vector2 start, Vector2 end)
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

		Vector2 normalizedVector = (start - end).normalized;
		float distance = Vector2.Distance (start, end);
		int numSegments = Mathf.RoundToInt(distance / segmentWidth);

		for (int i = 0; i < numSegments; i ++)
		{
			GameObject o = Instantiate (boltPrefab);
			Vector2 pos = start + (normalizedVector * -i * segmentWidth) - (normalizedVector * segmentWidth / 2f);
			o.transform.position = pos;
			o.transform.parent = this.transform;

			float angle = Mathf.Atan2 (normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
			o.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));

			o.GetComponent<SpriteRenderer> ().flipX = Random.value < 0.5f;
		}
	}
}
