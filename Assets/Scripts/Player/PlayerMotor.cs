using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerMotor : EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private Camera cam = null;
    private NetworkRigidbody networkRigidbody = null;

    private float speed = 7f;

    private Vector3 lastServerPos = Vector3.zero;
    private bool firstState = true;

    private bool jumpPressed = false;
    private float jumpForce = 9f;

    private bool isGrounded = false;
    private float maxAngle = 45f;

    [SerializeField]
    private int totalLife = 250;

    private SphereCollider headCollider;

    public int TotalLife => totalLife;

    private void Awake()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();
        headCollider = GetComponent<SphereCollider>();
    }

    public void Init(bool isMine)
    {
        if (isMine)
            cam.gameObject.SetActive(true);
    }

    public State ExecutedCommand(bool forward, bool backward, bool left, bool right, bool jump, float yaw, float pitch)
    {
        Vector3 movingDir = Vector3.zero;
        if (forward ^ backward)
        {
            movingDir += forward ? transform.forward : -transform.forward;
        }
        if (left ^ right)
        {
            movingDir += right ? transform.right : -transform.right;
        }

        if (jump)
        {
            if (jumpPressed == false && isGrounded)
            {
                isGrounded = false;
                jumpPressed = true;
                networkRigidbody.MoveVelocity += Vector3.up * jumpForce;
            }
        }
        else
        {
            if (jumpPressed)
                jumpPressed = false;
        }

        movingDir.Normalize();
        movingDir *= speed;
        networkRigidbody.MoveVelocity = new Vector3(movingDir.x, networkRigidbody.MoveVelocity.y, movingDir.z);

        cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        if (entity.IsOwner)
        {
            state.Pitch = (int) pitch;
        }

        State stateMotor = new State();
        stateMotor.position = transform.position;
        stateMotor.rotation = yaw;

        return stateMotor;
    }

    public void SetPitch()
    {
        if (!entity.IsControllerOrOwner)
            cam.transform.localEulerAngles = new Vector3(state.Pitch, 0f, 0f);
    }

    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsControllerOrOwner)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.3f))
                {
                    float slopeNormal =
                        Mathf.Abs(Vector3.Angle(hit.normal, new Vector3(hit.normal.x, 0, hit.normal.z)) - 90) % 90;
                    if (networkRigidbody.MoveVelocity.y < 0)
                        networkRigidbody.MoveVelocity = Vector3.Scale(networkRigidbody.MoveVelocity, new Vector3(1, 0, 1));

                    if (!isGrounded && slopeNormal <= maxAngle)
                    {
                        isGrounded = true;
                    }
                }
                else
                {
                    if (isGrounded)
                    {
                        isGrounded = false;
                    }
                }
            }
        }
    }

    public void SetState(Vector3 position, float rotation)
    {
        if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
            transform.rotation = Quaternion.Euler(0, rotation, 0);

        if (firstState) // TODO need read why first
        {
            if (position != Vector3.zero)
            {
                transform.position = position;
                firstState = false;
                lastServerPos = Vector3.zero;
            }
        }
        else
        {
            if (position != Vector3.zero)
            {
                lastServerPos = position;
            }

            transform.position += (lastServerPos - transform.position) * 0.5f;
        }
    }

    public bool IsHeadshot(Collider c)
    {
        return c == headCollider;
    }

    public void Life(PlayerMotor killer, int life)
    {
        if (entity.IsOwner)
        {
            int value = state.LifePoints + life;

            if (value < 0)
            {
                state.LifePoints = 0;
            }else if(value > totalLife)

            {
                state.LifePoints = totalLife;
            }
            else
            {
                state.LifePoints = value;
            }
        }
    }

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }
}
