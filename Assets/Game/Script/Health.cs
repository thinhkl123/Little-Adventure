using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public float currentHealthPercentage
    {
        get
        {
            return (float) currentHealth/ (float) maxHealth;
        }
    }
    private Character _cc;

    private void Awake()
    {
        currentHealth = maxHealth;
       _cc = GetComponent<Character>(); 
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        CheckHealth();
    }

    public void AddHealth(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void CheckHealth()
    {
        if (currentHealth <=0)
        {
            _cc.SwitchToState(Character.CharacterState.Dead);
        }
    }
}
