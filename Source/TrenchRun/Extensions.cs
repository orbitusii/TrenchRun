// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun;

/// <summary>
/// Extensions Script.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Checks to see if a Collider's attached rigidbody has the specified <paramref name="tag"/>
    /// </summary>
    /// <param name="actor">A <see cref="PhysicsColliderActor"/>, usually retrieved through a raycast, collision, or trigger event</param>
    /// <param name="tag">The tag to check for</param>
    /// <returns>True if the collider's attached rigidbody has the specified tag<br/>
    /// False if the collider's rigidbody doesn't have the tag, or there is no attached rigidbody for the collider</returns>
    public static bool AttachedRigidbodyHasTag(this PhysicsColliderActor actor, string tag)
    {
        Tag t = Tags.Get(tag);

        return AttachedRigidbodyHasTag(actor, t);
    }

    /// <summary>
    /// Checks to see if a Collider's attached rigidbody has the specified <paramref name="tag"/>
    /// </summary>
    /// <param name="actor">A <see cref="PhysicsColliderActor"/>, usually retrieved through a raycast, collision, or trigger event</param>
    /// <param name="tag">The tag to check for</param>
    /// <returns>True if the collider's attached rigidbody has the specified tag<br/>
    /// False if the collider's rigidbody doesn't have the tag, or there is no attached rigidbody for the collider</returns>
    public static bool AttachedRigidbodyHasTag(this PhysicsColliderActor actor, Tag tag)
    {
        if (actor.AttachedRigidBody is null) return false;

        return actor.AttachedRigidBody.HasTag(tag);
    }
}
