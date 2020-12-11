using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : ObservableRatio {
	private float maxHealth;
	private float damageTaken;

	public void Initialize(float maxHealthAmount) {
		maxHealth = maxHealthAmount;
		damageTaken = 0f;
	}

	public float GetHealth() {
		return maxHealth - damageTaken;
	}

	public override float GetRatio() {
		return (float)(GetHealth () / maxHealth);
	}

	public float GetAbsoluteAmount() {
		return GetHealth ();
	}

	public void Damage (float amount) {
		if (amount == 0) return;
		damageTaken += amount;
		if (damageTaken > maxHealth) damageTaken = maxHealth;
		NotifyObservers ();
	}

}
