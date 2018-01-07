using UnityEngine;
using System.Collections;

public class EnemyDetectionZoneEmitter : MonoBehaviour
{
	public GameObject zonePrefab;
	public EnemyDetectionZone.OnDetectEnemyCallback onDetectEnemy;

	public enum TraceMode {
		Path,
		TimeInterval
	}
	public TraceMode traceMode;
	[Tooltip("Only used if traceMode is TimeInterval")]
	public float timeInterval;

	void Start()
	{
		switch (traceMode)
		{
			case TraceMode.Path:
				StartCoroutine(EmitRoutinePath());
				break;
			case TraceMode.TimeInterval:
				StartCoroutine(EmitRoutineTimeInterval());
				break;
		}
		
	}

	private IEnumerator EmitRoutinePath()
	{
		CreateZone();
		Vector3 lastEmittedPosition = transform.position;
		for (;;)
		{
			float distanceTraveled = Vector3.Distance(transform.position, lastEmittedPosition);
			if (distanceTraveled >= zonePrefab.GetComponent<CircleCollider2D>().radius * 2f)
			{
				CreateZone();
				lastEmittedPosition = transform.position;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator EmitRoutineTimeInterval()
	{
		for (;;)
		{
			CreateZone();
			yield return new WaitForSeconds(timeInterval);
		}
	}

	private void CreateZone()
	{
		GameObject o = Instantiate(zonePrefab, transform.position, Quaternion.identity) as GameObject;
		o.transform.SetParent(ObjectPooler.GetObjectPooler("Effect").transform);
		o.GetComponent<EnemyDetectionZone>().SetOnDetectEnemyCallback(onDetectEnemy);
	}
}

