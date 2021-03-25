using System;
using Bolt;
using UnityEngine;

public class PlayerController : EntityBehaviour<IPhysicState>
{
    private PlayerMotor playerMotor;
    private bool forward;
    private bool backward;
    private bool left;
    private bool right;
    private float yaw;
    private float pitch;
    private bool hasControl;
    private float mouseSensitivity = 5f;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        hasControl = entity.HasControl;
        Init(hasControl);
        playerMotor.Init(hasControl);
    }

    public void Init(bool isMine)
    {
        if (isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            FindObjectOfType<PlayerSetupController>().SceneCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (hasControl)
            PollKeys();
    }

    private void PollKeys()
    {
        forward = Input.GetKey(KeyCode.W);
        backward = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw %= 360;
        pitch += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);
    }

    public override void SimulateController()
    {
        IPlayerCommandInput input = PlayerCommand.Create();
        input.Forward = forward;
        input.Backward = backward;
        input.Left = left;
        input.Right = right;
        input.Yaw = yaw;
        input.Pitch = pitch;

        entity.QueueInput(input);
        playerMotor.ExecutedCommand(forward, backward, left, right, yaw, pitch);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand) command;

        if (resetState) //TODO need read
        {
            playerMotor.SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {
            PlayerMotor.State motorState = new PlayerMotor.State();

            if (!entity.HasControl)
            {
                motorState = playerMotor.ExecutedCommand(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right,
                    cmd.Input.Yaw, cmd.Input.Pitch);
            }

            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = motorState.rotation;
        }
    }
}