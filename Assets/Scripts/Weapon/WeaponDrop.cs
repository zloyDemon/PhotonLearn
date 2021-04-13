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

    private WeaponDropToken dropToken = null;
    private PlayerMotor launcher = null;

    [SerializeField]
    private GameObject render = null;
    private BoxCollider boxCollider = null;
    private SphereCollider sphereCollider = null;
    private float time = 0;

    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        inited = true;
    }

    private void Awake()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        time = Time.time + 2f;
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

        dropToken = (WeaponDropToken) entity.AttachToken;
        launcher = BoltNetwork.FindEntity(dropToken.networkId).GetComponent<PlayerMotor>();
        inited = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (entity.IsAttached)
        {
            if (entity.IsOwner && (inited || !collision.gameObject.GetComponent<PlayerMotor>()))
                networkRigidbody.MoveVelocity *= 0.5f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inited && entity.IsAttached && entity.IsOwner)
        {
            if (other.GetComponent<PlayerMotor>())
            {
                if (other.GetComponent<PlayerWeapons>().CanAddWeapon(dropToken.ID))
                {
                    if (other.GetComponent<PlayerMotor>() == launcher && time < Time.time)
                    {
                        other.GetComponent<PlayerWeapons>().AddWeaponEvent((int)dropToken.ID, dropToken.currentAmmo, dropToken.totalAmmo);
                        BoltNetwork.Destroy(entity);
                        networkRigidbody.enabled = false;
                        boxCollider.enabled = false;
                        render.SetActive(false);
                        sphereCollider.enabled = false;
                    }
                    else if (other.GetComponent<PlayerMotor>() != launcher)
                    {
                        other.GetComponent<PlayerWeapons>().AddWeaponEvent((int)dropToken.ID, dropToken.currentAmmo, dropToken.totalAmmo);
                        BoltNetwork.Destroy(entity);
                        networkRigidbody.enabled = false;
                        boxCollider.enabled = false;
                        render.SetActive(false);
                        sphereCollider.enabled = false;
                    }
                }
            }
        }
    }
}
