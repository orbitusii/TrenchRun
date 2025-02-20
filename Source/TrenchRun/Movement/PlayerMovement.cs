// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace TrenchRun.Movement;

/// <summary>
/// PlayerMovement Script.
/// </summary>
[RequireActor(typeof(RigidBody))]
public class PlayerMovement : Script
{
    public float ForwardSpeed = 2;
    public float StrafeSpeed = 1;

    public float RotationSpeed = 1;

    public float AimScanDistance;
    public LayersMask AimScanLayers;

    private RigidBody rb => Actor as RigidBody ?? throw new Exception("PlayerMovement script is attached to an Actor that isn't a Rigidbody! The script won't work like this.");

    private Actor? AimTarget = null;
    private Vector3? AimPosition = null;

    public bool InvertAimedPitch = false;
    public bool InvertAimedRoll = false;

    public override void OnFixedUpdate()
    {
        DoMovement();
        DoAiming();
    }

    private void DoMovement()
    {
        bool isPreciseAim = Input.GetAction("Aim");
        float fwdBack = Input.GetAxis("Trigger R") - Input.GetAxis("Trigger L");
        float strafeVert = Input.GetAxis("Vertical");
        float strafeHorz = Input.GetAxis("Horizontal");
        float aimVert = Input.GetAxis("Mouse Y");
        float aimHorz = Input.GetAxis("Mouse X");
        Vector3 motion = Vector3.Zero;

        if(isPreciseAim)
        {
            if (AimPosition is null)
            {
                bool hitsomething = Physics.RayCast(Actor.Position, Actor.Transform.Forward, out RayCastHit hit, AimScanDistance, AimScanLayers, hitTriggers: false);

                if (hitsomething) ResolveHit(hit);
                else AimPosition = Actor.Position + Actor.Transform.Forward * AimScanDistance;
            }

            // Update AimPosition if the target is an aim target
            AimPosition = AimTarget?.Position ?? AimPosition;
            //Debug.Log(AimPosition);
        }
        else if (AimPosition is not null)
        {
            AimPosition = null;
            AimTarget = null;
        }

        motion = new Vector3(strafeHorz * StrafeSpeed, strafeVert * StrafeSpeed, fwdBack * StrafeSpeed);
        motion = Vector3.ClampLength(motion, StrafeSpeed);

        motion.Z = fwdBack > 0 ? fwdBack * ForwardSpeed : motion.Z;

        Vector3 worldMotion = Actor.Transform.LocalToWorldVector(motion);

        rb.AddMovement(worldMotion);
    }

    private void ResolveHit(RayCastHit hit)
    {
        Tag t = Tags.Get("AimTarget");

        if (hit.Collider.HasTag(t) || (hit.Collider.AttachedRigidBody is not null && hit.Collider.AttachedRigidBody.HasTag(t)))
        {
            AimTarget = hit.Collider.AttachedRigidBody;
            if (AimTarget is null) AimTarget = hit.Collider;
        }
        else AimPosition = hit.Point;
    }

    private void DoAiming()
    {
        bool isPreciseAim = Input.GetAction("Aim");
        float fwdBack = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");
        float aimVert = Input.GetAxis("Mouse Y");
        float aimHorz = Input.GetAxis("Mouse X");

        if (isPreciseAim && AimPosition is not null)
        {
            Actor.Orientation = Quaternion.LookRotation((AimPosition ?? throw new NullReferenceException()) - Actor.Position, Actor.Transform.Up) * Quaternion.Euler(0, 0, (InvertAimedRoll ? -aimHorz : aimHorz) * RotationSpeed * 10 * Time.DeltaTime * Mathf.Pi);
        }
        else
        {
            Vector3 rotationAxis = new Vector3(aimVert, aimHorz, 0);
            Quaternion pitch = Quaternion.RotationAxis(Vector3.Right, aimVert * RotationSpeed * Time.DeltaTime * Mathf.Pi);
            Quaternion yaw = Quaternion.RotationAxis(Vector3.Up, aimHorz * RotationSpeed * Time.DeltaTime * Mathf.Pi);

            Actor.Orientation = Actor.Orientation * pitch * yaw;
        }
    }
}
