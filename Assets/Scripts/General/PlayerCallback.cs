using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerCallback : EntityEventListener<IPlayerState>
{
    private PlayerMotor playerMotor;
    private PlayerWeapons playerWeapons;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerWeapons = GetComponent<PlayerWeapons>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);
        state.AddCallback("Pitch", playerMotor.SetPitch);

        if (entity.IsOwner)
        {
            state.LifePoints = playerMotor.TotalLife;
        }
    }

    public void FireEffect(float precision, int seed)
    {
        FireEffectEvent evnt = FireEffectEvent.Create(entity, EntityTargets.EveryoneExceptController);
        evnt.Precision = precision;
        evnt.Seed = seed;
        evnt.Send();
    }

    public override void OnEvent(FireEffectEvent evnt)
    {
        playerWeapons.FireEffect(evnt.Seed, evnt.Precision);
    }

    private void UpdatePlayerLife()
    {
        if (entity.HasControl)
            GUIController.Current.UpdateLife(state.LifePoints, playerMotor.TotalLife);
    }
}
