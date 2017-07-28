using UnityEngine;

public class MageFireOrbs : HeroPowerUp
{
	private int maxOrbs = 3;
	private int orbRequirement = 10;
	public int orbsActive { get; private set; }
	public IndicatorEffect[] orbs;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		hero.player.OnEnemyDamaged += UpdateCombo;
		hero.player.OnPlayerDamaged += AbsorbDamage;
	}

	public override void Stack()
	{
		base.Stack();
		maxOrbs++;
	}

	private void UpdateCombo(float num)
	{
		if (playerHero.combo != 0 && playerHero.combo % orbRequirement == 0)
		{
			ActivateOrb();
		}
	}

	private void ActivateOrb()
	{
		if (orbsActive >= maxOrbs)
			return;

		orbs[orbsActive].gameObject.SetActive(true);
		orbsActive++;
	}

	public void AbsorbDamage(int amt)
	{
		if (orbsActive <= 0)
			return;

		playerHero.player.Heal(Mathf.CeilToInt(amt / 2f));
		DeactivateOrb();
	}

	public void DeactivateOrb()
	{
		orbs[orbsActive - 1].AnimateOut();
		orbsActive--;
	}
}