// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.Hazards;

/// <summary>
/// Script that spins a hazard object constantly at a specific speed.
/// </summary>
public class Spinner : Script
{
    /// <summary>
    /// The axis around which the hazard is rotated
    /// </summary>
    public Vector3 RotationAxis = Vector3.Up;

    /// <summary>
    /// How long it takes the hazard to complete a full rotation. Must be greater than zero.
    /// </summary>
    public float RotationPeriod = 4;

    /// <summary>
    /// The temporal offset for this spinner, in seconds (i.e. the spinner will start with a rotation offset equal to how far it would spin in N seconds).
    /// </summary>
    public float StartTimeOffset;

    public Quaternion additiveRotation => RotationPeriod > 0 ? Quaternion.RotationAxis(RotationAxis.Normalized, Mathf.TwoPi / RotationPeriod * Time.DeltaTime) : Quaternion.RotationAxis(RotationAxis, Mathf.TwoPi * Time.DeltaTime);

    public override void OnDebugDrawSelected()
    {
        DebugDraw.DrawLine(Actor.Position, Actor.Position + RotationAxis * 100, Color.Blue);
    }

    public override void OnStart()
    {
        if (RotationPeriod <= 0) return;

        Quaternion warmupRotation = Quaternion.RotationAxis(RotationAxis.Normalized, Mathf.TwoPi / RotationPeriod * StartTimeOffset);
        Actor.LocalOrientation = Actor.LocalOrientation * warmupRotation;
    }

    public override void OnFixedUpdate()
    {
        Actor.LocalOrientation = Actor.LocalOrientation * additiveRotation;

        base.OnFixedUpdate();
    }
}
