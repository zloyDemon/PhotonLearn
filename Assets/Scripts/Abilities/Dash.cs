using System.Collections;
using UnityEngine;

public class Dash : Ability
{
    private NetworkRigidbody networkRigidbody = null;
    [SerializeField]
    private Transform cam;
    private float dashForce = 40f;
    private float dashDuration = 1f;
    private bool dashing = false;

    private void Awake()
    {
        _cooldown = 2;
        networkRigidbody = GetComponent<NetworkRigidbody>();
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
            _Dash();
        }

        if (dashing)
        {
            networkRigidbody.MoveVelocity = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized * dashForce;
        }
    }

    private void _Dash()
    {
        if (entity.IsOwner)
            state.Energy -= _cost;
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
        dashing = true;
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
    }
}
