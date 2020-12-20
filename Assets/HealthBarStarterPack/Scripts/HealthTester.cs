using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthTester : MonoBehaviour
{
    private Health health;
    private TextMeshProUGUI energyTXT;
    public float curHealth;
    [SerializeField]
    private GameObject healthBarPrefab;
    public static bool recoverEnergy = false;
    void Start()
    {
        health = transform.GetComponent<Health>();
        health.Initialize(100f);
        curHealth = health.GetHealth();
        BarUI barUi = GameObject.Instantiate(healthBarPrefab, transform).GetComponent<BarUI>();
        barUi.transform.localPosition = new Vector3(0f, .4f, 0f);
        barUi.Initialize(health);
        energyTXT = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        // if (Input.GetKey (KeyCode.Space)) {
        // 	float mult = 10f;
        // 	float dmg = (Time.deltaTime * mult);
        // 	health.Damage (dmg);
        // }

        if (recoverEnergy)
        {
            float mult = 30f;
            float change = (Time.deltaTime * mult);
            curHealth = 100;
            if (health.GetHealth() + change > curHealth)
            {
                change = curHealth - health.GetHealth();
            }
            health.Damage(-change);
            energyTXT.text = (int)health.GetHealth() + "/100";
            if (health.GetHealth() == curHealth)
            {
                recoverEnergy = false;
            }
        }
        else if (health.GetHealth() > curHealth)
        {
            float mult = 10f;
            float change = (Time.deltaTime * mult);
            if (health.GetHealth() - change < curHealth)
            {
                change = health.GetHealth() - curHealth;
            }
            health.Damage(change);
            if (health.GetHealth() > 0 && health.GetHealth() < 1)
            {
                energyTXT.text = "1/100";
            }
            else
            {
                energyTXT.text = (int)health.GetHealth() + "/100";
            }
        }
    }

    public bool consumeEnergy(float amount)
    {
        if (curHealth > 0)
        {
            if (curHealth - amount < 0)
            {
                curHealth = 0;
            }
            else
            {
                curHealth -= amount;
            }
            return true;
        }

        return false;
    }
}
