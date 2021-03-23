using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Bolt.Matchmaking;
using Bolt.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadlessServerManager : Bolt.GlobalEventListener
{
    [SerializeField] private string map = "";
    [SerializeField] private string roomId = "Test";
    [SerializeField] private bool isServer = false;

    private static string sMap;
    private static string sRoomId;

    public bool IsServer
    {
        get => isServer;
        set => isServer = value;
    }

    public static string RoomId()
    {
        return sRoomId;
    }

    public static string Map()
    {
        return sMap;
    }

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            PhotonRoomProperties roomProperties = new PhotonRoomProperties();
            roomProperties.AddRoomProperty("m", map);
            roomProperties.IsOpen = true;
            roomProperties.IsVisible = true;

            if (sRoomId.Length == 0)
            {
                sRoomId = Guid.NewGuid().ToString();
            }

            BoltMatchmaking.CreateSession(sRoomId, roomProperties, map);
        }
    }

    private void Start()
    {
        isServer = "true" == (GetArg("-s", "-isServer") ?? (isServer ? "true" : "false"));
        sMap = GetArg("-m", "-map") ?? map;
        sRoomId = GetArg("-r", "-room") ?? roomId;

        if (IsServer)
        {
            var validMap = false;

            foreach (var value in BoltScenes.AllScenes)
            {
                if (SceneManager.GetActiveScene().name != value)
                {
                    if (sMap == value)
                    {
                        validMap = true;
                        break;
                    }
                }
            }

            if (!validMap)
            {
                BoltLog.Error("Invalid configuration: please verify level name");
                Application.Quit();
            }

            BoltLauncher.StartServer();
            DontDestroyOnLoad(this);
        }
    }

    static string GetArg(params string[] names)
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            foreach (var name in names)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }

        return null;
    }
}
