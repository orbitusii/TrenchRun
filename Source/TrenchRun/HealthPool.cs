// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;
using TrenchRun.CollisionTools;

namespace TrenchRun;

/// <summary>
/// A pool of health for a single unit or object in the world - depleting its health will cause the object to be considered destroyed.
/// </summary>
public class HealthPool : Script
{
    public Collider? Hitbox { get; set; }

    public bool Alive { get; private set; } = true;

    public int MaxHealth = 100;
    public int CurrentHealth = 100;

    public override void OnAwake()
    {
        if (Hitbox is IHitbox hb) hb.OnDamage += ApplyDamage;
    }

    public void ApplyDamage (object? sender, IHitbox.HitData hitdata)
    {
        if (!Alive) return;

        var contributor = Find<Actor>(ref hitdata.OwnerID);

        Debug.Log($"{Actor.Name} took {hitdata.DamageAmount} damage from {contributor?.Name ?? hitdata.OwnerID.ToString()}");
        CurrentHealth -= hitdata.DamageAmount;

        if(CurrentHealth <= 0)
        {
            Debug.Log($"{Actor.Name} was killed by {contributor?.Name ?? hitdata.OwnerID.ToString()}");
            Alive = false;
        }
    }

    public override void OnDestroy()
    {
        if (Hitbox is IHitbox hb) hb.OnDamage -= ApplyDamage;
    }
}
