using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private UICooldown skill = null;
    [SerializeField]
    private UICooldown grenade = null;
    [SerializeField]
    private Text energyCount = null;

    public UICooldown Skill { get => skill; }
    public UICooldown Grenade { get => grenade; }

    private void Start()
    {
        Show(false);
    }

    public void Show(bool active)
    {
        uiHealthBar.gameObject.SetActive(active);
        uiAmmoPanel.gameObject.SetActive(active);
        skill.gameObject.SetActive(active);
        grenade.gameObject.SetActive(active);
        energyCount.gameObject.SetActive(active);
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

    public void UpdateAbilityView(int i)
    {
        energyCount.text = i.ToString();
        skill.UpdateCost(i);
        grenade.UpdateCost(i);
    }
}
