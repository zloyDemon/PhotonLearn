using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponDrop : EntityBehaviour<IPhysicState>
{
    private NetworkRigidbody networkRigidbody;
    private bool inited = false;

    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        inited = true;
    }

    private void Awake()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();
    }

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            if (transform.rotation == Quaternion.identity)
            {
                networkRigidbody.MoveVelocity = Random.onUnitSphere * 10f;
                transform.eulerAngles = Random.insideUnitSphere * 360f;
            }
            else
            {
                networkRigidbody.MoveVelocity = transform.forward * 10f;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (entity.IsAttached)
        {
            if (entity.IsOwner && (inited || !collision.gameObject.GetComponent<PlayerMotor>()))
                networkRigidbody.MoveVelocity *= 0.5f;
        }
    }
}
