using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : Ability
{
    [SerializeField] private Transform _cam = null;
    [SerializeField] private LayerMask _layerMask;
    private float _maxDistance = 5f;

    private void Awake()
    {
        _cooldown = 10;
        _uiCooldown = GUIController.Current.Skill;
        _uiCooldown.InitView(_abilityInterval);
        _cost = 1;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            _timer = BoltNetwork.ServerFrame;

            if(entity.HasControl)
                _uiCooldown.StartCooldown();

            if (entity.IsOwner)
            {
                state.Energy -= _cost;

                RaycastHit hit;
                if (Physics.Raycast(_cam.position, _cam.transform.forward, out hit, _maxDistance, _layerMask))
                {
                    BoltNetwork.Instantiate(BoltPrefabs.Medkit, hit.point, Quaternion.identity).GetComponent<AQE>()
                        .launcher = GetComponent<PlayerMotor>();
                }
                else
                {
                    BoltNetwork.Instantiate(BoltPrefabs.Medkit, transform.position, Quaternion.identity)
                        .GetComponent<AQE>().launcher = GetComponent<PlayerMotor>();
                }
            }
        }
    }
}
