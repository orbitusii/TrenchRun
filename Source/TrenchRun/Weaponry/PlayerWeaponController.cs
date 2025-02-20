// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.Weaponry;

/// <summary>
/// Controller that translates input into Weapon behavior
/// </summary>
public class PlayerWeaponController : Script
{
    public string FireAction = "Fire";

    public WeaponBase? Weapon;

    public override void OnEnable()
    {
        Input.ActionTriggered += Input_ActionTriggered;

        base.OnEnable();
    }

    private void Input_ActionTriggered(string arg1, InputActionState arg2)
    {
        Action? call = null;

        if (arg1.Equals(FireAction))
        {
            call = arg2 switch
            {
                InputActionState.Press => ShootStart,
                InputActionState.Release => ShootEnd,
                _ => null
            };
        }

        call?.Invoke();
    }

    private void ShootStart ()
    {
        Weapon?.ShootOnce();
        Weapon?.StartShooting();
    }

    private void ShootEnd()
    {
        Weapon?.StopShooting();
    }

    public override void OnDisable()
    {
        Input.ActionTriggered -= Input_ActionTriggered;
        base.OnDisable();
    }
}
