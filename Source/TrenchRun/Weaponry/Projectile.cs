// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;
using TrenchRun.CollisionTools;

namespace TrenchRun.Weaponry;

/// <summary>
/// Base class for Projectiles - can be derived to implement unique behavior, such as bouncing when impacting walls!
/// </summary>
public class Projectile : Script
{
    /// <summary>
    /// The Actor that owns this projectile. I.e. if this is a player or enemy or hazard etc.
    /// </summary>
    public Guid Owner { get; private set; }

    /// <summary>
    /// How fast this projectile will travel
    /// </summary>
    public float Speed;

    /// <summary>
    /// How much damage this projectile causes to Hitboxes that it collides with.
    /// </summary>
    public int Damage;

    /// <summary>
    /// How many seconds before this projectile is deactivated automatically.
    /// </summary>
    public float MaximumLifetime = 5;

    /// <summary>
    /// The collider that this projectile should use for raycasting, if any.
    /// </summary>
    public Collider? Collider;

    /// <summary>
    /// Layers this projectile should interact with.
    /// </summary>
    public LayersMask Layers;

    private Vector3 PoolStoragePosition;
    private bool ShouldDeactivateRootActor = false;

    public bool IsFlying { get; private set; } = false;
    private float ActivateTime = -10000;
    private Vector3 velocity = Vector3.Zero;

    private IHitbox.HitData HitData => new IHitbox.HitData(Owner, Damage);

    /// <summary>
    /// Method that initializes this Projectile for use in an ObjectPool.
    /// </summary>
    /// <param name="storagePosition">The world position that this projectile will be stored at when deactivated</param>
    /// <param name="deactivateActorOnStore">Should this projectile deactivate its root Actor when deactivated?</param>
    public void InitializePooledProjectile(Vector3 storagePosition, bool deactivateActorOnStore)
    {
        PoolStoragePosition = storagePosition;
        ShouldDeactivateRootActor = deactivateActorOnStore;
    }

    public void Activate(Vector3 position, Vector3 direction, Guid owner)
    {
        Actor.IsActive = true;
        IsFlying = true;
        ActivateTime = Time.GameTime;
        Owner = owner;

        Actor.Position = position;
        Actor.Orientation = Quaternion.LookRotation(direction);
        velocity = direction.Normalized * Speed;

        if (Collider is not null)
        {
            Collider.IsActive = true;
        }
    }

    public override void OnFixedUpdate()
    {
        if (!IsFlying) return;

        bool hitSomething = Physics.RayCast(Actor.Position, velocity.Normalized, out RayCastHit hit, Speed * Time.DeltaTime, Layers, hitTriggers: false);

        if (hitSomething)
        {
            bool shouldContinue = OnHitObject(hit.Collider);

            if (!shouldContinue) Deactivate();
        }
        else if (Time.GameTime >= ActivateTime + MaximumLifetime) Deactivate();
        else Actor.Position += velocity * Time.DeltaTime;

        base.OnFixedUpdate();
    }

    /// <summary>
    /// Method called when a projectile hits an object. Return value indicates whether this projectile should continue to fly or is deactivated.
    /// </summary>
    /// <param name="hit">The collider that was hit</param>
    /// <returns>True if the projectile can continue flying<br/>
    /// False if the projectile should deactivate</returns>
    public virtual bool OnHitObject(PhysicsColliderActor hit)
    {
        //Debug.Log($"Projectile {Actor.Name} hit {hit.Name}");
        if (hit is IHitbox hb)
            ApplyDamage(hb);

        return false;
    }

    protected void ApplyDamage(IHitbox hitbox)
    {
        hitbox.ApplyHit(HitData);
    }

    private void Deactivate()
    {
        if (Collider is not null)
        {
            Collider.IsActive = true;
        }

        if (ShouldDeactivateRootActor)
            Actor.IsActive = false;

        IsFlying = false;
        Actor.Position = PoolStoragePosition;
    }
}
