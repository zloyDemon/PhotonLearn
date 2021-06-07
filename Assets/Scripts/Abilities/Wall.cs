﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Ability
{
    [SerializeField]
    private Transform _cam = null;
    [SerializeField]
    private LayerMask _layerMask = 0;
    private static float MAX_DISTANCE = 9f;
    private static float VERTICAL_THRESHOLD = 0.4f;

    [SerializeField]
    private GameObject _wallPreset = null;
    private GameObject _wallInstantiated = null;

    [SerializeField]
    private GameObject _stateMachine = null;
    private GameObject _preview = null;
    private bool _previewMode = false;

    private MeshRenderer _renderer;
    private bool _tooFar = false;

    private Color _red;
    private Color _blue;

    private RaycastHit _hit;

    private void Awake()
    {
        _cooldown = 10;
        _red = new Color(255 / 255f, 81 / 255f, 0, 40 / 255f);
        _blue = new Color(0, 183 / 255f, 255 / 255f, 40 / 255f);
        _uiCooldown = GUIController.Current.Skill;
        _uiCooldown.InitView(_abilityInterval);
        _cost = 1;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);

        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out _hit, Mathf.Infinity, _layerMask))
            {
                if (entity.HasControl)
                {
                    if (_preview == null)
                    {
                        _preview = GameObject.Instantiate(_stateMachine, _hit.point, _cam.transform.rotation);
                        _renderer = _preview.GetComponent<MeshRenderer>();
                        _renderer.material.SetColor("_Color", _blue);
                    }
                    else
                    {
                        if (!_tooFar)
                        {
                            _uiCooldown.StartCooldown();
                            _timer = BoltNetwork.ServerFrame;
                        }
                        _tooFar = false;
                        GameObject.Destroy(_preview);
                    }
                }

                if (entity.IsOwner)
                {
                    if (_previewMode)
                    {
                        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out _hit, Mathf.Infinity, _layerMask))
                        {
                            if (_hit.distance < MAX_DISTANCE)
                            {
                                state.Energy -= _cost;
                                _timer = BoltNetwork.ServerFrame;

                                if (_wallInstantiated != null)
                                    BoltNetwork.Destroy(_wallInstantiated);
                                _wallInstantiated = BoltNetwork.Instantiate(_wallPreset);
                                _wallInstantiated.transform.rotation = transform.rotation;
                                _wallInstantiated.transform.position = _hit.point;

                            }
                        }
                    }
                    _previewMode ^= true;
                }
            }
        }
    }

    private void Update()
    {
        if (_preview != null)
        {
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out _hit, Mathf.Infinity, _layerMask))
            {
                _preview.transform.rotation = transform.rotation;

                _preview.transform.position = _hit.point;
                _preview.transform.Translate(Vector3.up * 2f);

                if (_hit.normal.y > VERTICAL_THRESHOLD)
                {
                    if ((_hit.distance > MAX_DISTANCE) != _tooFar)
                    {
                        _tooFar ^= true;
                        _renderer.material.SetColor("_Color", (_tooFar) ? _red : _blue);
                    }
                }
                else
                {
                    _tooFar = true;
                    _renderer.material.SetColor("_Color", _red);
                }
            }
        }
    }
}
