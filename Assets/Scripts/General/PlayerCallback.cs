using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerCallback : EntityEventListener<IPlayerState>
{
    private PlayerMotor playerMotor;
    private PlayerWeapons playerWeapons;
    private PlayerController playerController;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerWeapons = GetComponent<PlayerWeapons>();
        playerController = GetComponent<PlayerController>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);
        state.AddCallback("Pitch", playerMotor.SetPitch);
        state.AddCallback("WeaponIndex", UpdateWeaponIndex);
        state.AddCallback("Weapons[].CurrentAmmo", UpdateWeaponAmmo);
        state.AddCallback("Weapons[].TotalAmmo", UpdateWeaponAmmo);

        if (entity.IsOwner)
        {
            state.LifePoints = playerMotor.TotalLife;
        }
    }

    private void UpdateWeaponAmmo(IState state1, string propertypath, ArrayIndices arrayindices)
    {
        int index = arrayindices[0];
        IPlayerState s = (IPlayerState) state;
        playerWeapons.InitAmmo(index, s.Weapons[index].CurrentAmmo, s.Weapons[index].TotalAmmo);
    }

    private void UpdateWeaponIndex()
    {
        playerController.Wheel = state.WeaponIndex;
        playerWeapons.SetWeapon(state.WeaponIndex);
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
