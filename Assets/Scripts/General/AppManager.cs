using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    [SerializeField] private HeadlessServerManager headlessServerManager = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!headlessServerManager.IsServer)
        {
            SceneManager.LoadScene("menu");
        }
    }
}
