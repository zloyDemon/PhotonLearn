using System;
using Bolt;
using UnityEngine;
using Random = System.Random;

public class PlayerController : EntityBehaviour<IPhysicState>
{
    private PlayerMotor playerMotor;
    private PlayerWeapons playerWeapons;
    private bool forward;
    private bool backward;
    private bool left;
    private bool right;
    private float yaw;
    private float pitch;
    private bool jump;

    private bool fire;
    private bool aiming;
    private bool reload;
    private int wheel = 0;
    private bool drop;
    private bool ability1;
    private bool ability2;

    private bool hasControl;
    private float mouseSensitivity = 5f;

    public int Wheel
    {
        get => wheel;
        set => wheel = value;
    }

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerWeapons = GetComponent<PlayerWeapons>();
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        if (entity.HasControl)
        {
            hasControl = true;
            GUIController.Current.Show(true);
        }

        Init(hasControl);
        playerMotor.Init(hasControl);
        playerWeapons.Init();
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
        jump = Input.GetKey(KeyCode.Space);

        fire = Input.GetMouseButton(0);

        aiming = Input.GetMouseButton(1);
        reload = Input.GetKey(KeyCode.R);
        drop = Input.GetKey(KeyCode.G);

        ability1 = Input.GetKey(KeyCode.Q);
        ability2 = Input.GetKey(KeyCode.E);

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw %= 360;
        pitch += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            wheel = playerWeapons.CalculateIndex(Input.GetAxis("Mouse ScrollWheel"));
        }
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
        input.Jump = jump;
        input.Drop = drop;
        input.Ability1 = ability1;
        input.Ability2 = ability2;

        input.Fire = fire;
        input.Scope = aiming;
        input.Reload = reload;
        input.Wheel = wheel;

        entity.QueueInput(input);
        playerMotor.ExecutedCommand(forward, backward, left, right, jump, yaw, pitch, ability1, ability2);
        playerWeapons.ExecuteCommand(fire, aiming, reload, wheel, BoltNetwork.ServerFrame % 1024, drop);
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
                motorState = playerMotor.ExecutedCommand(
                    cmd.Input.Forward,
                    cmd.Input.Backward,
                    cmd.Input.Left,
                    cmd.Input.Right,
                    cmd.Input.Jump,
                    cmd.Input.Yaw,
                    cmd.Input.Pitch,
                    cmd.Input.Ability1,
                    cmd.Input.Ability2);

                playerWeapons.ExecuteCommand(
                    cmd.Input.Fire,
                    cmd.Input.Scope,
                    cmd.Input.Reload,
                    cmd.Input.Wheel,
                    cmd.ServerFrame % 1024,
                    cmd.Input.Drop);
            }

            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = motorState.rotation;
        }
    }
}