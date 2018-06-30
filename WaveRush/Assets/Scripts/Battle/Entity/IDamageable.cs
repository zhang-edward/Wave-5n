using UnityEngine;

public interface IDamageable {

	void Damage(int amt, IDamageable source);
	void Heal(int amt);
}
