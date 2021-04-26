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

    private int weaponIndex = 1;
    private bool dropPressed;

    [SerializeField] private WeaponId primaryWeapon = WeaponId.None;
    [SerializeField] private WeaponId secondaryWeapon = WeaponId.None;

    private WeaponId primary = WeaponId.None;
    private WeaponId secondary = WeaponId.None;

    public int WeaponIndex => weaponIndex;

    public Camera Cam { get => cam; }

    [SerializeField] private Transform weaponTransform = null;
    [SerializeField] private GameObject[] weaponPrefabs = null;

    public void Init()
    {
        if (entity.IsOwner)
        {
            for (int i = 0; i < 4; i++)
            {
                state.Weapons[i].CurrentAmmo = -1;
            }

            AddWeaponEvent(primaryWeapon);
            AddWeaponEvent(secondaryWeapon);
        }

        StartCoroutine(SetWeapon());
        weapons[0].Init(this, 0);
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload, int wheel, int seed, bool drop)
    {
        if (wheel != state.WeaponIndex)
        {
            if (weapons[wheel] != null)
                if(entity.IsOwner)
                    state.WeaponIndex = wheel;
        }

        if (weapons[weaponIndex])
            weapons[weaponIndex].ExecuteCommand(fire, aiming, reload, seed);

        DropCurrent(drop);
    }

    public void FireEffect(int seed, float precision)
    {
        weapons[weaponIndex].FireEffect(seed, precision);
    }

    public void InitAmmo(int i, int current, int total)
    {
        if(weapons[i] && i != 0)
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

        while (weapons[i] == null)
        {
            i += factor;
            i = i % weapons.Length;
        }

        return i;
    }

    public void DropCurrent(bool drop)
    {
        if (drop)
        {
            if (!dropPressed)
            {
                dropPressed = true;
                DropWeapon();
                if (entity.IsOwner)
                    state.WeaponIndex = CalculateIndex(1);
            }
        }
        else
        {
            if (dropPressed)
            {
                dropPressed = false;
            }
        }
    }

    public void DropWeapon()
    {
        if (entity.IsOwner)
        {
            WeaponDropToken token = new WeaponDropToken();
            token.currentAmmo = weapons[weaponIndex].CurrentAmmo;
            token.totalAmmo = weapons[weaponIndex].TotalAmmo;
            token.ID = weapons[weaponIndex].WeaponStat.ID;
            token.networkId = entity.NetworkId;

            BoltNetwork.Instantiate(weapons[weaponIndex].WeaponStat.drop, token,
                Cam.transform.position + Cam.transform.forward, Quaternion.LookRotation(Cam.transform.forward));

            if (weapons[weaponIndex].WeaponStat.ID < WeaponId.SecondaryEnd)
                secondary = WeaponId.None;
            else
                primary = WeaponId.None;

            state.Weapons[weaponIndex].ID = -1;
            Destroy(weapons[weaponIndex].gameObject);
            weapons[weaponIndex] = null;
        }
    }

    public void RemoveWeapon(int i)
    {
        if(weapons[i])
            Destroy(weapons[i].gameObject);

        weapons[i] = null;
    }

    public bool CanAddWeapon(WeaponId toAdd)
    {
        if (toAdd < WeaponId.SecondaryEnd)
        {
            if (secondary == WeaponId.None)
                return true;
        }
        else
        {
            if (primary == WeaponId.None)
                return true;
        }

        return false;
    }

    public void AddWeaponEvent(int i, int ca, int ta)
    {
        if (i < (int) WeaponId.SecondaryEnd)
        {
            state.Weapons[1].ID = i;
            state.Weapons[1].CurrentAmmo = ca;
            state.Weapons[1].TotalAmmo = ta;
        }
        else
        {
            state.Weapons[2].ID = i;
            state.Weapons[2].CurrentAmmo = ca;
            state.Weapons[2].TotalAmmo = ta;
        }
    }

    public void AddWeapon(WeaponId id)
    {
        if (id == WeaponId.None)
            return;

        GameObject prefab = null;
        foreach (var w in weaponPrefabs)
        {
            if (w.GetComponent<Weapon>().WeaponStat.ID == id)
            {
                prefab = w;
                break;
            }
        }

        prefab = Instantiate(prefab, weaponTransform.position, Quaternion.LookRotation(weaponTransform.forward),
            weaponTransform);

        if (id < WeaponId.SecondaryEnd)
        {
            secondary = id;
            weapons[1] = prefab.GetComponent<Weapon>();
            prefab.GetComponent<Weapon>().Init(this, 1);
        }
        else
        {
            primary = id;
            weapons[2] = prefab.GetComponent<Weapon>();
            prefab.GetComponent<Weapon>().Init(this, 2);
        }
    }

    public void AddWeaponEvent(WeaponId id)
    {
        if (id == WeaponId.None)
            return;

        int i = (id < WeaponId.SecondaryEnd) ? 1 : 2;
        state.Weapons[i].ID = (int) id;
    }

    IEnumerator SetWeapon()
    {
        while (weapons[weaponIndex] == null)
            yield return new WaitForEndOfFrame();
        SetWeapon(weaponIndex);
    }
}
