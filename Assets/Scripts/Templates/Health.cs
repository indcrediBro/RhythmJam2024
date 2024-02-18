using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] protected int m_startingHealth = 100;
    [SerializeField] protected int m_currentHealth;

    public virtual int GetMaxHealthValue() { return m_startingHealth; }
    public virtual int GetCurrentHealthValue() { return m_currentHealth; }

    public virtual void TakeDamage(int _damage)
    {
        m_currentHealth -= _damage;
        if(m_currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void AddHealth(int _health)
    {
        if (m_currentHealth + _health <= m_startingHealth)
        {
            m_currentHealth += _health;
        }
        else if (m_currentHealth + _health > m_startingHealth)
        {
            ResetHealth();
        }
    }

    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }

    private void ResetHealth()
    {
        m_currentHealth = m_startingHealth;

    }

    protected void OnEnable()
    {
        ResetHealth();
    }
}
