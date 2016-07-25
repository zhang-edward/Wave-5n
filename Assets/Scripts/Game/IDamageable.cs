using UnityEngine;
using System.Collections;

public interface IDamageable {

	void Damage(int amt);
	void Heal(int amt);
}
