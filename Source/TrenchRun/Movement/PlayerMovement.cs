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
    public float PitchLimit = 89;

    public float AimScanDistance;
    public LayersMask AimScanLayers;

    private RigidBody rb => Actor as RigidBody ?? throw new Exception("PlayerMovement script is attached to an Actor that isn't a Rigidbody! The script won't work like this.");

    private Actor? AimTarget = null;
    private Vector3? AimPosition = null;

    public bool InvertAimedPitch = false;
    public bool InvertAimedRoll = false;

    private Vector3 aimVector;

    public override void OnStart()
    {
        aimVector = Actor.Transform.Forward;
    }

    public override void OnFixedUpdate()
    {
        DoMovement();
        DoAiming();
    }

    private void DoMovement()
    {
        bool isPreciseAim = Input.GetAction("Aim");
        float fwdBack = Input.GetAxis("Vertical");
        float strafeVert = Input.GetAxis("Trigger R") - Input.GetAxis("Trigger L");
        float strafeHorz = Input.GetAxis("Horizontal");
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

        Quaternion heading = Quaternion.Euler(0, Actor.EulerAngles.Y, 0);
        Vector3 worldMotion = Vector3.Transform(motion, heading);

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
            Actor.Orientation = Quaternion.LookRotation((AimPosition ?? throw new NullReferenceException()) - Actor.Position, Vector3.Up);
            //* Quaternion.Euler(0, 0, (InvertAimedRoll ? -aimHorz : aimHorz) * RotationSpeed * 10 * Time.DeltaTime * Mathf.Pi);

            aimVector = Actor.Transform.Forward;
        }
        else
        {
            // * Mathf.Cos(Actor.EulerAngles.X * Mathf.DegreesToRadians)

            float pitchAngle = Actor.EulerAngles.X;
            float clampedAimVert = Mathf.Clamp(aimVert, pitchAngle <= -PitchLimit ? 0 : -1, pitchAngle >= PitchLimit ? 0 : 1);

            Quaternion pitch = Quaternion.RotationAxis(Actor.Transform.Right, clampedAimVert * RotationSpeed * Time.DeltaTime * Mathf.Pi);
            Quaternion yaw = Quaternion.RotationAxis(Vector3.Up, aimHorz * RotationSpeed * Time.DeltaTime * Mathf.Pi);

            Vector3 newAimVector = Vector3.Transform(aimVector, pitch * yaw);

            Quaternion oldOrientation = Actor.Orientation;
            Quaternion newOrientation = Quaternion.LookRotation(newAimVector, Vector3.Up);

            if (Mathf.Abs(newOrientation.Y - newOrientation.Y) > 90)
            {
                newOrientation = oldOrientation;
                newAimVector = Vector3.Transform(Vector3.Forward, oldOrientation);
            }

            Actor.Orientation = newOrientation;

            aimVector = newAimVector;
        }
    }
}
