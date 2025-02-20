// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.Weaponry;

/// <summary>
/// Script that represents a single weapon
/// </summary>
public class WeaponBase : Script
{
    public Actor? Owner;
    public Vector3 ShotOrigin;
    public Vector3 WorldShotOrigin => Transform.LocalToWorld(ShotOrigin);

    /// <summary>
    /// Projectile prefab used for this weapon. Will be spawned using an ObjectPool.
    /// </summary>
    public Prefab? Projectile;

    /// <summary>
    /// Called when the player or AI first presses the "Fire" button or chooses to shoot, and only ONCE per press
    /// </summary>
    public virtual void ShootOnce() { }

    /// <summary>
    /// Called when the player or AI first presses the "Fire" button or chooses to shoot. Should be used to start cyclic shooting.
    /// </summary>
    public virtual void StartShooting()
    {
        Debug.Log("Player started shooting");
    }

    /// <summary>
    /// Called when the player or AI first releases the "Fire" button or chooses to stop shooting. Should be used to stop cyclic shooting.
    /// </summary>
    public virtual void StopShooting()
    {
        Debug.Log("Player started shooting");
    }
}
