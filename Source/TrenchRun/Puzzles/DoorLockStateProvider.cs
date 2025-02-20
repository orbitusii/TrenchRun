// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.Puzzles;

/// <summary>
/// DoorLockStateProvider Script.
/// </summary>
public class DoorLockStateProvider : Script
{
    public DoorLockState LockState { get; protected set; } = DoorLockState.Unlocked;
    [ShowInEditor, NoSerialize, ReadOnly]
    public string DebugDisplay => LockState.ToString();

    [property: ShowInEditor]
    public bool ForceLocked
    {
        get => LockState != 0;
        set
        {
            LockState = value ? DoorLockState.FullyLocked : DoorLockState.Unlocked;

            var e = OnLockStateChanged;
            e?.Invoke(this, LockState);
        }
    }

    /// <summary>
    /// Sets the specified Lock <paramref name="channel"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="channel">The channel, between 0 and 8 (corresponding to A through H) to set.</param>
    /// <param name="value">The value of the channel.</param>
    public void SetLockState (byte channel, bool value)
    {
        DoorLockState field = channel switch
        {
            0 => DoorLockState.Lock_A,
            1 => DoorLockState.Lock_B,
            2 => DoorLockState.Lock_C,
            3 => DoorLockState.Lock_D,
            4 => DoorLockState.Lock_E,
            5 => DoorLockState.Lock_F,
            6 => DoorLockState.Lock_G,
            7 => DoorLockState.Lock_H,
            _ => throw new ArgumentOutOfRangeException($"Lock State Channel {channel} is out of range. Should be from 0-7 (corresponding to A through H)")
        };

        // Either ORs the field if <value> is TRUE to combine them,
        // or XORs the field if <value> is FALSE to set that field to 0.
        /* example:
         *  field => Lock_B
         *  value = false
         *  LockState = Lock_A, Lock_B, Lock_C
         *  
         *  beep boop magic code and shit
         *  
         *  LockState becomes Lock_A, Lock_C as Lock_B was un-set by this operation.
        */
        LockState = value
            ? LockState | field
            : LockState ^ field;

        var e = OnLockStateChanged;
        e?.Invoke(this, LockState);
    }

    public event EventHandler<DoorLockState>? OnLockStateChanged;
}

/// <summary>
/// Enum representing the various states of a Door lock.<br/>
/// Contains channels for 8 distinct lock mechanisms (carried as flags), such as separate buttons, groups of enemies AND buttons, timers, etc.
/// </summary>
[Flags]
public enum DoorLockState: byte
{
    Unlocked = 0b0,
    Lock_A = 0b0000_0001,
    Lock_B = 0b0000_0010,
    Lock_C = 0b0000_0100,
    Lock_D = 0b0000_1000,
    Lock_E = 0b0001_0000,
    Lock_F = 0b0010_0000,
    Lock_G = 0b0100_0000,
    Lock_H = 0b1000_0000,
    FullyLocked = 0b1111_1111,
}
