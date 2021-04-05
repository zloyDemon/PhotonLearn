using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField]
    private Camera cam = null;
    [SerializeField]
    private Weapon weapons = null;

    public Camera Cam { get => cam; }

    public void Init()
    {
        weapons.Init(this);
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload, int seed)
    {
        weapons.ExecuteCommand(fire, aiming, reload, seed);
    }

    public void FireEffect(int seed, float precision)
    {
        weapons.FireEffect(seed, precision);
    }
}
