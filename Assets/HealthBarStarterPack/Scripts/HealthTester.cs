using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthTester : MonoBehaviour {
	private Health health;
	private TextMeshProUGUI energyTXT;
	private float curHealth;
	[SerializeField]
	private GameObject healthBarPrefab;
	void Start () {
		health = transform.GetComponent<Health> ();
		health.Initialize (100f);
		curHealth = health.GetHealth();
		BarUI barUi = GameObject.Instantiate(healthBarPrefab, transform).GetComponent<BarUI> ();
		barUi.transform.localPosition = new Vector3 (0f, .4f, 0f);
		barUi.Initialize (health);
		energyTXT = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
	}
	void Update() {
		// if (Input.GetKey (KeyCode.Space)) {
		// 	float mult = 10f;
		// 	float dmg = (Time.deltaTime * mult);
		// 	health.Damage (dmg);
		// }
		if(health.GetHealth() > curHealth){
			float mult = 10f;
			float dmg = (Time.deltaTime * mult);
			if(health.GetHealth() - dmg < curHealth){
				dmg = health.GetHealth() - curHealth;
			}
			health.Damage (dmg);
			energyTXT.text = (int)health.GetHealth() + "/100";
		}
	}

	public void consumeEnergy(float amount){
		curHealth -= amount;
	}
}
