// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.CollisionTools;

[ActorToolbox("Hitboxes")]
public class SphereHitbox : SphereCollider, IHitbox
{
    public event EventHandler<IHitbox.HitData>? OnDamage;

    public void ApplyHit(IHitbox.HitData hitData)
    {
        var e = OnDamage;
        e?.Invoke(this, hitData);
    }
}

[ActorToolbox("Hitboxes")]
public class CubeHitbox : BoxCollider, IHitbox
{
    public event EventHandler<IHitbox.HitData>? OnDamage;

    public void ApplyHit(IHitbox.HitData hitData)
    {
        var e = OnDamage;
        e?.Invoke(this, hitData);
    }
}

[ActorToolbox("Hitboxes")]
public class CapsuleHitbox : CapsuleCollider, IHitbox
{
    public event EventHandler<IHitbox.HitData>? OnDamage;

    public void ApplyHit(IHitbox.HitData hitData)
    {
        var e = OnDamage;
        e?.Invoke(this, hitData);
    }
}

[ActorToolbox("Hitboxes")]
public class MeshHitbox : MeshCollider, IHitbox
{
    public event EventHandler<IHitbox.HitData>? OnDamage;

    public void ApplyHit(IHitbox.HitData hitData)
    {
        var e = OnDamage;
        e?.Invoke(this, hitData);
    }
}