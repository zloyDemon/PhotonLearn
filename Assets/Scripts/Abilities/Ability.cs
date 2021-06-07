using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Ability : EntityEventListener<IPlayerState>
{
    protected bool _pressed = false;
    protected bool _buttonUp;
    protected bool _buttonDown;

    protected int _cooldown = 0;
    protected float _timer = 0;
    protected int _cost = 0;
    protected UICooldown _uiCooldown;

    protected int _abilityInterval => _cooldown * BoltNetwork.FramesPerSecond;

    public virtual void UpdateAbility(bool button)
    {
        _buttonUp = false;
        _buttonDown = false;
        if (button)
        {
            if (!_pressed)
            {
                _pressed = true;
                _buttonDown = true;
            }
        }
        else
        {
            if (_pressed)
            {
                _pressed = false;
                _buttonUp = true;
            }
        }
    }

    public virtual void ShowVisualEffect() { }
}
