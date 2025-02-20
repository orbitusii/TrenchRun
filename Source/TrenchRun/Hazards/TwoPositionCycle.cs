// Connieworks 2025

using System;
using System.Collections.Generic;
using System.Reflection;
using FlaxEngine;

namespace TrenchRun.Hazards;

/// <summary>
/// Script that drives a hazard object back and forth between two positions ad infinitum.
/// </summary>
public class TwoPositionCycle : Script
{
    /// <summary>
    /// The second position this object will move to, relative to its current (starting) position
    /// </summary>
    public Vector3 SecondPosition;

    /// <summary>
    /// How long it takes this object to move between its two positions
    /// </summary>
    public float TravelTime = 5;

    /// <summary>
    /// How long this object pauses at each end of its movement
    /// </summary>
    public float RestTime = 0;

    public float TimeOffset = 0;
    public int StateOffset = 0;

    private Vector3? startPosition;
    private float Timer = 0;
    private int state = 0;

    public override void OnDebugDraw()
    {
        Vector3 start = Actor.Parent.Transform.LocalToWorld(startPosition ?? Actor.LocalPosition);
        Vector3 end = Actor.Parent.Transform.LocalToWorld((startPosition ?? Actor.LocalPosition) + SecondPosition);

        DebugDraw.DrawLine(start, end, Color.HotPink);

        base.OnDebugDraw();
    }

    public override void OnDebugDrawSelected()
    {
        base.OnDebugDrawSelected();
    }

    public override void OnStart()
    {
        startPosition = Actor.LocalPosition;

        Timer = TimeOffset;
        state = StateOffset;

        base.OnStart();
    }

    public override void OnFixedUpdate()
    {
        Func<bool> motion = state switch
        {
            1 => MoveForward,
            3 => MoveBack,
            0 or 2 or _ => Wait
        };

        bool done = motion.Invoke();

        if (done)
        {
            state = ++state % 4;
            Timer = state % 2 != 0 ? TravelTime : RestTime;
        }
        else Timer -= Time.DeltaTime;

        base.OnFixedUpdate();
    }

    private bool MoveForward()
    {
        if (startPosition is null) return true;

        float t = (TravelTime - Timer) / TravelTime;

        Vector3 start = startPosition ?? throw new NullReferenceException();
        Vector3 end = (startPosition ?? throw new NullReferenceException()) + SecondPosition;

        Actor.LocalPosition = Vector3.Lerp(start, end, t);

        return Timer <= 0;
    }

    private bool Wait()
    {
        return Timer <= 0;
    }

    private bool MoveBack()
    {
        if (startPosition is null) return true;

        float t = 1 - (TravelTime - Timer) / TravelTime;

        Vector3 start = startPosition ?? throw new NullReferenceException();
        Vector3 end = (startPosition ?? throw new NullReferenceException()) + SecondPosition;

        Actor.LocalPosition = Vector3.Lerp(start, end, t);

        return Timer <= 0;
    }
}
