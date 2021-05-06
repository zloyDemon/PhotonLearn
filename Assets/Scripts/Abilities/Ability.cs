using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Ability : EntityEventListener<IPlayerState>
{
    protected bool pressed = false;
    protected bool buttonUp;
    protected bool buttonDown;

    protected int cooldown = 0;
    protected float timer = 0;
    protected int cost = 0;
    protected UICooldown uiCooldown;

    protected int abilityInterval => cooldown * BoltNetwork.FramesPerSecond;

    public virtual void UpdateAbility(bool button)
    {
        buttonUp = false;
        buttonDown = false;
        if (button)
        {
            if (!pressed)
            {
                pressed = true;
                buttonDown = true;
            }
        }
        else
        {
            if (pressed)
            {
                pressed = false;
                buttonUp = true;
            }
        }
    }

    public virtual void ShowVisualEffect() { }
}
