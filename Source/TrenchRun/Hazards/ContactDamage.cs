// Connieworks 2025

using System;
using System.Collections.Generic;
using System.Threading;
using FlaxEngine;
using TrenchRun.CollisionTools;

namespace TrenchRun.Hazards;

/// <summary>
/// Hazard that provides constant damage while a Hitbox is in contact with it.
/// </summary>
public class ContactDamage : Script
{
    /// <summary>
    /// The actor that should be credited with damage (usually the prefab root for any Hazards)
    /// </summary>
    public Actor? DamageOwner;

    /// <summary>
    /// How much damage is dealt by this hazard each tick
    /// </summary>
    public int DamagePerTick = 5;
    /// <summary>
    /// How many times per second this hazard "ticks" i.e. applies damage.
    /// </summary>
    public float TickRate = 5;
    public float SecondsPerTick => 1 / TickRate;

    private List<IHitbox> InHazard = new List<IHitbox>();
    private float timeToTick = 0;

    public override void OnEnable()
    {
        if (Actor is Collider c)
        {
            c.TriggerEnter += TriggerEntered;
            c.TriggerExit += TriggerExited;
        }
    }

    private void TriggerEntered(PhysicsColliderActor obj)
    {
        if (obj is IHitbox hitbox)
        {
            //Debug.Log(hitbox);
            InHazard.Add(hitbox);
        }
    }

    private void TriggerExited(PhysicsColliderActor obj)
    {
        if (obj is IHitbox hitbox)
        {
            //Debug.Log(hitbox);
            InHazard.Remove(hitbox);
        }
    }

    public override void OnFixedUpdate()
    {
        bool isTickTime = timeToTick <= 0;

        if(isTickTime)
        {
            IHitbox.HitData hazardHit = new IHitbox.HitData(DamageOwner?.ID ?? Actor.ID, DamagePerTick);
            timeToTick = SecondsPerTick;

            foreach(var hitbox in InHazard)
            {
                hitbox.ApplyHit(hazardHit);
            }

            return;
        }

        timeToTick -= Time.DeltaTime;
    }

    public override void OnDisable()
    {
        if (Actor is Collider c)
        {
            c.TriggerEnter -= TriggerEntered;
            c.TriggerExit -= TriggerExited;
        }
    }
}
