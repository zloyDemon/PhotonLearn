using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [SerializeField] private UIHealthBar uiHealthBar;
    [SerializeField] private UIAmmoPanel uiAmmoPanel;

    private static GUIController instance = null;

    public static GUIController Current
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GUIController>();
            return instance;
        }
    }

    private void Start()
    {
        Show(false);
    }

    public void Show(bool active)
    {
        uiHealthBar.gameObject.SetActive(active);
        uiAmmoPanel.gameObject.SetActive(active);
    }

    public void UpdateLife(int current, int total)
    {
        uiHealthBar.UpdateLife(current, total);
    }

    public void UpdateAmmo(int current, int total)
    {
        if (!uiAmmoPanel.gameObject.activeSelf)
           uiAmmoPanel.gameObject.SetActive(true);
        uiAmmoPanel.UpdateAmmo(current, total);
    }

    public void HideAmmo()
    {
        uiAmmoPanel.gameObject.SetActive(false);
    }
}
