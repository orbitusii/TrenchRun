// Connieworks 2025

using System;
using System.Collections.Generic;
using FlaxEngine;
using TrenchRun.Weaponry;

namespace TrenchRun;

/// <summary>
/// ObjectPool Script.
/// </summary>
public class ObjectPool : EmptyActor
{
    public Vector3 StoragePosition = new Vector3(0, -5000, 0);
    public bool ShouldDeactivateRoots = false;

    public Prefab? ObjectType;
    public int Quantity = 1;

    private Actor[] Objects = Array.Empty<Actor>();
    private int NextIndex = 0;

    public bool IsEmpty => Objects.Length < 1;

    public void InitializePool (Prefab obj, int quantity)
    {
        ObjectType = obj;
        Quantity = quantity;

        SpawnObjects();
    }

    public override void OnBeginPlay()
    {
        if(ObjectType is not null)
        {
            SpawnObjects();
        }

        base.OnBeginPlay();
    }

    private void SpawnObjects ()
    {
        if (ObjectType is null) return;

        Objects = new Actor[Quantity];
        Transform t = new Transform(StoragePosition);
        bool lacksScript = false;

        for (int i = 0; i < Quantity; i++)
        {
            var newobj = PrefabManager.SpawnPrefab(ObjectType, this, t);
            newobj.Name += $"#{i:000}";

            if (lacksScript) continue;

            if (newobj.GetScript<Projectile>() is Projectile ps)
            {
                ps.InitializePooledProjectile(StoragePosition, ShouldDeactivateRoots);
            }
            else lacksScript = true;

            Objects[i] = newobj;
        }
    }

    public Actor GetNextInstance ()
    {
        Actor a = Objects[NextIndex];

        if(++NextIndex >= Objects.Length) NextIndex = 0;

        return a;
    }

    public override void OnEndPlay()
    {
        base.OnEndPlay();
    }
}
