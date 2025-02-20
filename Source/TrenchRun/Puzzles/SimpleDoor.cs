// Connieworks 2025

using FlaxEngine;

namespace TrenchRun.Puzzles;

/// <summary>
/// A simple door implementation that instantly activate and deactivates an actor with a collider as its opening method.
/// </summary>
public class SimpleDoor : DoorBase
{
    /// <summary>
    /// The actor that will be activated and deactivated based on this door's
    /// </summary>
    public Actor? DoorActor;

    protected override void OnOpen()
    {
        if (DoorActor is null) return;

        DoorActor.IsActive = false;
    }

    protected override void OnClose()
    {
        if (DoorActor is null) return;

        DoorActor.IsActive = true;
    }
}
