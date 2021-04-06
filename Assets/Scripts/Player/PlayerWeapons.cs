using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerWeapons : EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private Camera cam = null;
    [SerializeField]
    private Weapon[] weapons = null;

    private int weaponIndex = 0;

    public int WeaponIndex => weaponIndex;

    public Camera Cam { get => cam; }

    public void Init()
    {
        foreach (var weapon in weapons)
        {
            if(weapon)
                weapon.Init(this);
        }

        SetWeapon(weaponIndex);
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload, int wheel, int seed)
    {
        if (wheel != state.WeaponIndex)
        {
            if (weapons[wheel] != null)
                if(entity.IsOwner)
                    state.WeaponIndex = wheel;
        }

        if (weapons[weaponIndex])
            weapons[weaponIndex].ExecuteCommand(fire, aiming, reload, seed);
    }

    public void FireEffect(int seed, float precision)
    {
        weapons[weaponIndex].FireEffect(seed, precision);
    }

    public void InitAmmo(int i, int current, int total)
    {
        weapons[i].InitAmmo(current, total);
    }

    public void SetWeapon(int index)
    {
        weaponIndex = index;

        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i] != null)
                weapons[i].gameObject.SetActive(false);
        }

        weapons[weaponIndex].gameObject.SetActive(true);
    }

    public int CalculateIndex(float valueToAdd)
    {
        int i = weaponIndex;
        int factor = 0;

        if (valueToAdd > 0)
            factor = 1;
        else if (valueToAdd < 0)
            factor = -1;

        i += factor;

        if (i == -1)
            i = weapons.Length - 1;

        if (i == weapons.Length)
            i = 0;

        return i;
    }
}
