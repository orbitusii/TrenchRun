// Connieworks 2025

using System;
using System.Collections.Generic;
using System.IO;

//using System.Text.RegularExpressions;
using FlaxEngine;

namespace TrenchRun.Weaponry;

/// <summary>
/// A simple weapon that shoots a single projectile repeatedly.<br/>
/// This one uses object pooling for hopefully improved performance.
/// </summary>
public class SimplePeashooter : WeaponBase
{
    /// <summary>
    /// Fire rate in rounds per minute
    /// </summary>
    public float FireRate = 60;

    /// <summary>
    /// Fire rate in rounds per second (1/60th of <see cref="FireRate"/>)
    /// </summary>
    public float FireRatePerSecond => FireRate / 60;

    /// <summary>
    /// Delay, in seconds, between successive shots (1/<see cref="FireRatePerSecond"/>)
    /// </summary>
    private float CyclicDelay => 1 / FireRatePerSecond;

    /// <summary>
    /// Scalar that reflects the projectile lifetime, in order to account for how long projectiles might live at most if fired into oblivion.
    /// </summary>
    public int PoolLifetimeScalar = 5;

    /// <summary>
    /// The maximum number of projectiles that can be fired from this Peashooter at once, accounting for their lifetime and fire rate
    /// </summary>
    public int PoolQuantity => Mathf.CeilToInt(FireRatePerSecond * PoolLifetimeScalar);

    /// <summary>
    /// Object Pool used to cache projectiles for this weapon for performance reasons.
    /// </summary>
    private ObjectPool? Pool;
    //private static readonly Regex prefabNameRegex = new Regex(@".*\/(?<name>.+)\.prefab");

    public bool IsShooting { get; private set; }
    private float lastShotTime = -100;

    public override void OnStart()
    {
        if (Projectile is null) return;

        var fullPath = Path.GetFileName(Projectile.Path);

        Pool = Scene.AddChild<ObjectPool>();
        Pool.Name = $"{Actor.Name}'s Projectile Pool - {fullPath} x{PoolQuantity}";

        Pool.InitializePool(Projectile, PoolQuantity);

        base.OnStart();
    }

    public override void StartShooting()
    {
        IsShooting = true;
    }

    public override void StopShooting()
    {
        IsShooting = false;
    }

    public override void OnFixedUpdate()
    {
        
        if (!IsShooting || Pool is null || Pool.IsEmpty) return;
        if(Time.GameTime >= lastShotTime + CyclicDelay)
        {
            Actor p = Pool.GetNextInstance();

            if(p.GetScript<Projectile>() is Projectile ps)
            {
                ps.Activate(WorldShotOrigin, Transform.Forward, Owner?.ID ?? Actor.ID);
            }

            lastShotTime = Time.GameTime;
        }

        base.OnFixedUpdate();
    }

    public override void OnDebugDrawSelected()
    {
        DebugDraw.DrawRay(new(WorldShotOrigin, Transform.Forward), Color.Orange, 100);

        base.OnDebugDrawSelected();
    }
}
