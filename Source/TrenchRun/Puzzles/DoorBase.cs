// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.Puzzles;

/// <summary>
/// DoorBase Script.
/// </summary>
public class DoorBase : Script
{
    /// <summary>
    /// A collider, set to act as a Trigger, to use for opening and closing this door automatically.
    /// </summary>
    public Collider? TriggerZone;

    /// <summary>
    /// Whether this door requires the presence of an object with the specified Door Opener Tag to be able to open.<br/>
    /// If True, at least one object with the Door Opener Tag must be inside of this door's Trigger Zone.<br/>
    /// If False, the door can open without any tagged objects inside the Trigger Zone.<br/>
    /// Setting this to false can be useful for doors that use a puzzle or timer to open.
    /// </summary>
    [Tooltip("True: at least one object with the Door Opener Tag must be inside of this door's Trigger Zone.\n" +
        "False: the door can open without any tagged objects inside the Trigger Zone.\n" +
        "Setting this to false can be useful for doors that use a puzzle or timer to open.")]
    public bool RequiresProximityToOpen = true;

    /// <summary>
    /// An instance of DoorLockStateProvider somewhere in the scene. The door will inherit this provider's LockState value.
    /// </summary>
    public DoorLockStateProvider? LockStateProvider;

    /// <summary>
    /// This door's lock state, either inherited from this door's LockStateProvider or, if no provider is specified, DoorLockState.Unlocked.
    /// </summary>
    public DoorLockState LockState => LockStateProvider?.LockState ?? DoorLockState.Unlocked;

    /// <summary>
    /// Whether this door *can* open, based on the combination of its proximity requirement and if the TriggerZone is occupied, plus its LockState.
    /// </summary>
    public bool CanOpen => LockState == 0 && (ZoneOccupied || !RequiresProximityToOpen);

    /// <summary>
    /// Should this door open? i.e., is there 1 or more objects with the "Door Opener" tag within this door's trigger zone?
    /// </summary>
    public bool ZoneOccupied => OpenersInZone.Count > 0;

    /// <summary>
    /// The tag that this door will look for when something enters its Trigger Zone in order to count it as an opener.
    /// </summary>
    public Tag DoorOpenerTag = Tags.Get("Door.Opener");

    /// <summary>
    /// A list of Object IDs representing the actors that have the specified Door Opener Tag that are also within this door's Trigger Zone.
    /// </summary>
    private List<Guid> OpenersInZone = new List<Guid>();

    /// <summary>
    /// If this door is currently open.
    /// </summary>
    public bool IsOpen { get; protected set; } = false;

    public override void OnAwake()
    {
        if(TriggerZone is not null)
        {
            TriggerZone.TriggerEnter += TriggerZone_TriggerEnter;
            TriggerZone.TriggerExit += TriggerZone_TriggerExit;
        }
        if (LockStateProvider is not null)
        {
            LockStateProvider.OnLockStateChanged += LockStateChanged;
        }

        base.OnAwake();
    }

    private void LockStateChanged(object? sender, DoorLockState e)
    {
        if (CanOpen) Open();
        else Close();
    }

    /// <summary>
    /// Called when something enters the TriggerZone. Adds the actor to the <see cref="OpenersInZone"/> list, if it has the specified <see cref="DoorOpenerTag"/>
    /// </summary>
    /// <param name="obj"></param>
    private void TriggerZone_TriggerEnter(PhysicsColliderActor obj)
    {
        if(obj.HasTag(DoorOpenerTag) || obj.AttachedRigidbodyHasTag(DoorOpenerTag))
        {
            OpenersInZone.Add(obj.ID);
        }

        if (CanOpen) Open();
    }

    /// <summary>
    /// Called when something leaves the TriggerZone. Removes the actor from the <see cref="OpenersInZone"/> list, if it's there.
    /// </summary>
    /// <param name="obj"></param>
    private void TriggerZone_TriggerExit(PhysicsColliderActor obj)
    {
        OpenersInZone.Remove(obj.ID);

        if(!CanOpen) Close();
    }

    /// <summary>
    /// Method called when all lock/trigger zone requirements are met in order to initiate door opening.
    /// </summary>
    protected void Open ()
    {
        if (IsOpen) return;

        Debug.Log("Door Opened!");
        IsOpen = true;

        OnOpen();
    }

    /// <summary>
    /// Called by <see cref="Open"/>. Implement this method to add custom opening behavior.
    /// </summary>
    protected virtual void OnOpen() { }

    /// <summary>
    /// Method called when any lock/trigger zone requirements are not met in order to initiate door closing.
    /// </summary>
    protected void Close ()
    {
        if (!IsOpen) return;

        Debug.Log("Door Closed!");
        IsOpen = false;

        OnClose();
    }

    /// <summary>
    /// Called by <see cref="Close"/>. Implement this method to add custom closing behavior.
    /// </summary>
    protected virtual void OnClose() { }

    public override void OnDestroy()
    {
        if (TriggerZone is not null)
        {
            TriggerZone.TriggerEnter -= TriggerZone_TriggerEnter;
            TriggerZone.TriggerExit -= TriggerZone_TriggerExit;
        }
        if (LockStateProvider is not null)
        {
            LockStateProvider.OnLockStateChanged -= LockStateChanged;
        }

        base.OnDestroy();
    }
}
