using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Element, ICombatant
{
    private float health = 100.0f;
    private float damage = 10.0f;
    public float Damage { get { return damage; } set { damage = value; } }

    public void DealDamage(ICombatant opponent)
    {
        opponent.TakeDamage(Damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public float GetHealth()
    {
        return health;
    }

    public string CombatName()
    {
        return "Enemy";
    }

    public bool IsEnemyAI()
    {
        return true;
    }
}
