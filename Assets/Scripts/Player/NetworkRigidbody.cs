using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class NetworkRigidbody : EntityBehaviour<IPhysicState>
{
    private Vector3 moveVelocity;
    private Rigidbody rb;
    [SerializeField] private float gravityForce = 1f;
    private bool useGravity = true;

    public Vector3 MoveVelocity
    {
        set
        {
            if (entity.IsControllerOrOwner)
            {
                moveVelocity = value;
            }
        }

        get => moveVelocity;
    }

    public float GravityForce => Physics.gravity.y * gravityForce * BoltNetwork.FrameDeltaTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsControllerOrOwner)
            {
                float g = moveVelocity.y;

                if (useGravity)
                {
                    if (moveVelocity.y < 0f)
                        g += 1.5f * GravityForce;
                    else if (moveVelocity.y > 0f)
                        g += 1f * GravityForce;
                    else
                        g = rb.velocity.y;
                }

                moveVelocity = new Vector3(moveVelocity.x, g, moveVelocity.z);
                rb.velocity = moveVelocity;
            }
        }
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }
}
