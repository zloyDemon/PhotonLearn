using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerCallback : EntityBehaviour<IPlayerState>
{
    private PlayerMotor playerMotor;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);

        if (entity.IsOwner)
        {
            state.LifePoints = playerMotor.TotalLife;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            state.LifePoints += 10;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            state.LifePoints -= 10;
    }

    private void UpdatePlayerLife()
    {
        if (entity.HasControl)
            GUIController.Current.UpdateLife(state.LifePoints, playerMotor.TotalLife);
    }
}
