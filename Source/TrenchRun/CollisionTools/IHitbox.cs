// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.CollisionTools;

/// <summary>
/// IHitbox Script.
/// </summary>
public interface IHitbox
{
    /// <summary>
    /// Event called when an object is hit by a projectile or hazard
    /// </summary>
    public event EventHandler<IHitbox.HitData>? OnDamage;

    /// <summary>
    /// Event called by projectiles when this hitbox is hit by them. Implementations should invoke the <see cref="OnDamage"/> event.
    /// </summary>
    /// <param name="hitData"></param>
    public void ApplyHit(HitData hitData);

    public class HitData
    {
        public Guid OwnerID;
        public int DamageAmount = 5;

        public HitData(Guid owner, int damage)
        {
            OwnerID = owner;
            DamageAmount = damage;
        }
    }
}
