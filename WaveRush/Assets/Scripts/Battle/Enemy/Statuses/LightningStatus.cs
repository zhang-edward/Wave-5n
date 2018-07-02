using UnityEngine;
using System.Collections;

public class LightningStatus : EnemyStatus
{
	public float lightningBounceRange = 5.0f;
	public int numBounces = 3;
	public GameObject lightningBoltPrefab;
	[Header("Arc Effect")]
	public SimpleAnimation[] lightningArcAnimations;
	public SimpleAnimationPlayer anim;
	public AudioClip[] applySounds;

	private ContinuousAnimatedLine lightning;
	private Transform effectsFolder;

	void Awake()
	{
		effectsFolder = ObjectPooler.GetObjectPooler ("Effect").transform;
		lightning = Instantiate (lightningBoltPrefab, effectsFolder).GetComponent<ContinuousAnimatedLine> ();
	}

	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		anim.transform.localScale = Vector3.one * enemy.statusIconSize;
	}

	protected override IEnumerator Effect ()
	{
		SoundManager.instance.RandomizeSFX (applySounds [Random.Range (0, applySounds.Length)]);
		InvokeRepeating ("LightningArcEffect", 0, 1.0f);
		yield return new WaitForSeconds (0.2f);
		while (timer >= 0)
		{
			//enemy.sr.color = Color.blue;
			if (numBounces > 0)
				Bounce ();
			yield return null;
		}
		//enemy.sr.color = Color.white;
		anim.enabled = false;

		Deactivate(1f);
	}

	private void LightningArcEffect()
	{
		anim.anim = lightningArcAnimations [Random.Range (0, lightningArcAnimations.Length)];
		anim.Play ();
	}

	private void Bounce()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, lightningBounceRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Enemy"))
			{
				Enemy e = col.GetComponentInChildren<Enemy> ();
				// check if the enemy already has this status
				bool hasLightning = e.GetStatus (statusName) != null;
				if (!hasLightning)
				{
					GameObject toAdd = Instantiate (this.gameObject);
					toAdd.GetComponent<LightningStatus> ().numBounces = numBounces - 1;
					e.AddStatus (toAdd);
					e.Damage (1, null);

					lightning.Init (transform.position, e.transform.position);

					numBounces = 0;
					timer = 0.5f;
					break;
				}
			}
		}
	}

	void OnDisable()
	{
		CancelInvoke ();
	}
}

