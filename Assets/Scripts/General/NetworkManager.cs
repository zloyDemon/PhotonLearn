using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : GlobalEventListener
{
    [SerializeField] private Text feedback;
    [SerializeField] private Button connectButton;


    private void Awake()
    {
        connectButton.onClick.AddListener(Connect);
    }

    public void FeedbackUser(string text)
    {
        feedback.text = text;
    }

    public void Connect()
    {
        FeedbackUser("Connecting..");
        BoltLauncher.StartClient();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching...");
        BoltMatchmaking.JoinSession(HeadlessServerManager.RoomId());
    }

    public override void Connected(BoltConnection connection)
    {
        FeedbackUser("Connected.");
    }
}
