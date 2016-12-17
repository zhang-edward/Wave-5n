using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFollow : MonoBehaviour
{
	private Transform target;
	private Vector3 offset;
	private RectTransform rect;

	void Awake()
	{
		rect = GetComponent<RectTransform> ();
	}

	public void Init(Transform target)
	{
		this.target = target;
		offset = Vector3.zero;
		transform.position = target.transform.position;
	}

	public void Init(Transform target, Vector3 offset)
	{
		this.target = target;
		this.offset = offset;
		transform.position = target.transform.position;
	}

	void Update()
	{
		if (target != null)
		{
			rect.anchoredPosition = target.transform.position + offset;
		}
	}
}

