using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NinjaShadowBackup : HeroPowerUp
{
	private const float RADIUS = 4f;

	private NinjaHero ninja;
	private int numCompanions = 1;
	private float activateChance = 0.4f;

	public GameObject companionPrefab;
	private List<GameObject> companions = new List<GameObject>();

	public SimpleAnimation[] companionAnimations;

	public override void Activate (PlayerHero hero)
	{
		base.Activate (hero);
		ninja = (NinjaHero)hero;
		ninja.OnNinjaDash += ShadowBackup;

		// instantiate companions
		for (int i = 0; i < numCompanions; i ++)
		{
			CreateCompanion ();
		}
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		ninja.OnNinjaDash -= ShadowBackup;
	}

	public override void Stack ()
	{
		base.Stack ();
		activateChance += 0.1f;
		numCompanions++;
		CreateCompanion ();
	}

	private void CreateCompanion()
	{
		Transform parent = ObjectPooler.GetObjectPooler ("Effect").transform;
		GameObject o = Instantiate (companionPrefab, parent);
		companions.Add (o);
		o.SetActive (false);
	}

	/*void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (transform.position, RADIUS);
	}*/

	public void ShadowBackup()
	{
		if (Random.value > activateChance)
			return;
		
		// get enemies in a radius and attack random 3
		int i = 0;
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, RADIUS);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				StartCoroutine(ActivateCompanion (e, i));
				i++;
				if (i >= companions.Count)
					break;
			}
		}
	}

	public IEnumerator ActivateCompanion(Enemy e, int i)
	{
		// random delay before starting routine
		yield return new WaitForSeconds (Random.Range(0f, 0.5f));

		// randomize the orientation of the sprite a bit (flip, position)
		SpriteRenderer sr = companions[i].GetComponent<SpriteRenderer> ();
		float f = UtilMethods.RandSign();
		Vector3 position = e.transform.position + new Vector3 (f * 0.5f, 0);	// offset sprite
		sr.flipX = f > 0;

		TempObject tempObj = companions[i].GetComponent<TempObject> ();
		SimpleAnimationPlayer anim = companions[i].GetComponent<SimpleAnimationPlayer> ();
		anim.anim = companionAnimations [Random.Range (0, companionAnimations.Length)];
		tempObj.Init (Quaternion.identity,
			position,
			anim.anim.frames [0]);
		anim.Play ();

		yield return new WaitForSeconds (0.2f);
		DamageEnemy (e, 1);
	}

	public void DamageEnemy(Enemy e, int damage)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			EffectPooler.PlayEffect(ninja.hitEffect, e.transform.position, true, 0.2f);

			SoundManager.instance.PlaySingle (ninja.dashOutSound);

			ninja.player.TriggerOnEnemyLastHitEvent (e);
		}
	}
}

