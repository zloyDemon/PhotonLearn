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
        cooldown = 2;
        networkRigidbody = GetComponent<NetworkRigidbody>();
        uiCooldown = GUIController.Current.Skill;
        uiCooldown.InitView(abilityInterval);
        cost = 1;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (buttonDown && timer + abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - cost) >= 0)
        {
            timer = BoltNetwork.ServerFrame;
            if(entity.HasControl)
                uiCooldown.StartCooldown();
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
            state.Energy -= cost;
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
        dashing = true;
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
    }
}
