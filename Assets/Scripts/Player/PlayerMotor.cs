using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam = null;
    private Rigidbody rigidbody = null;

    private float speed = 7f;

    private Vector3 lastServerPos = Vector3.zero;
    private bool firstState = true;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(bool isMine)
    {
        if (isMine)
            cam.gameObject.SetActive(true);
    }

    public State ExecutedCommand(bool forward, bool backward, bool left, bool right, float yaw, float pitch)
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

        movingDir.Normalize();
        movingDir *= speed;
        rigidbody.velocity = movingDir;

        cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        State stateMotor = new State();
        stateMotor.position = transform.position;
        stateMotor.rotation = yaw;

        return stateMotor;
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

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }
}
