using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupController : GlobalEventListener
{
    [SerializeField] private GameObject setupPanel;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            setupPanel.SetActive(true);
        }
    }

    private void Awake()
    {
        var button = setupPanel.GetComponentInChildren<Button>();
        button.onClick.AddListener(SpawnPlayer);
    }

    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player, Vector3.up, Quaternion.identity);
        entity.AssignControl(evnt.RaisedBy);
    }

    public void SpawnPlayer()
    {
        SpawnPlayerEvent evnt = SpawnPlayerEvent.Create(GlobalTargets.OnlyServer); // TODO GlobalTargets need read
        evnt.Send();
    }
}
