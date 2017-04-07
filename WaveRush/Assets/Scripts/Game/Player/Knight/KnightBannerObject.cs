using UnityEngine;

public class KnightBannerObject : MonoBehaviour
{
	public float radius;
	public float duration;
	public IndicatorEffect indicator;
	public Transform effectCircle;

	void OnEnable()
	{
		Invoke("AnimateOut", duration);
	}

	void Update()
	{
		effectCircle.localScale = Vector3.one * (radius / 1.5f);
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
				ApplyWeakness(e);
			}
		}
	}

	private void ApplyWeakness(Enemy e)
	{
		e.AddStatus(Instantiate(StatusEffectContainer.instance.GetStatus("Weakness")));
	}

	private void AnimateOut()
	{
		indicator.AnimateOut();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}