using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Weapon : MonoBehaviour
{
    protected Transform camera;
    [SerializeField] protected WeaponStats weaponStat = null;
    protected int currentAmmo = 0;
    protected int currentTotalAmmo = 0;
    protected bool isReloading = false;
    protected PlayerWeapons playerWeapons;
    protected PlayerMotor playerMotor;
    protected PlayerCallback playerCallback;

    protected int fireFrame = 0;
    private Coroutine reloadCrt = null;
    protected Dictionary<PlayerMotor, int> dmgCounter;

    [SerializeField] private GameObject renderer;
    [SerializeField] private Transform muzzle;

    protected int fireInterval
    {
        get
        {
            int rps = weaponStat.rpm / 60;
            return BoltNetwork.FramesPerSecond / rps;
        }
    }

    public WeaponStats WeaponStat => weaponStat;


    public int CurrentAmmo
    {
        get => currentAmmo;
        set
        {
            if (playerMotor.entity.IsOwner)
                playerMotor.state.Weapons[playerWeapons.WeaponIndex].CurrentAmmo = value;
            currentAmmo = value;
        }
    }

    public int TotalAmmo
    {
        get => currentTotalAmmo;
        set
        {
            if(playerMotor.entity.IsOwner)
                playerMotor.state.Weapons[playerWeapons.WeaponIndex].TotalAmmo = value;
        }
    }

    public virtual void Init(PlayerWeapons pw, int index)
    {
        playerWeapons = pw;
        playerMotor = pw.GetComponent<PlayerMotor>();
        playerCallback = pw.GetComponent<PlayerCallback>();
        camera = playerWeapons.Cam.transform;

        if (playerMotor.state.Weapons[index].CurrentAmmo != -1)
        {
            currentAmmo = playerMotor.state.Weapons[index].CurrentAmmo;
            currentTotalAmmo = playerMotor.state.Weapons[index].TotalAmmo;
        }
        else
        {
            currentAmmo = weaponStat.magazin;
            currentTotalAmmo = weaponStat.totalMagazin;
            if (playerMotor.entity.IsOwner)
            {
                playerMotor.state.Weapons[index].CurrentAmmo = currentAmmo;
                playerMotor.state.Weapons[index].TotalAmmo = currentTotalAmmo;
            }
        }

        if (playerMotor.entity.HasControl)
            renderer.gameObject.layer = 0;
    }

    private void OnEnable()
    {
        if (playerWeapons)
        {
            if (playerWeapons.entity.IsControllerOrOwner)
            {
                if(CurrentAmmo == 0)
                    Reload();
            }

            if (playerWeapons.entity.HasControl)
            {
                GUIController.Current.UpdateAmmo(CurrentAmmo, TotalAmmo);
            }
        }
    }

    private void OnDisable()
    {
        if (isReloading)
        {
            isReloading = false;
            StopCoroutine(reloadCrt);
        }
    }

    public virtual void ExecuteCommand(bool fire, bool aiming, bool reload, int seed)
    {
        if (!isReloading)
        {
            if (reload && CurrentAmmo != weaponStat.magazin && TotalAmmo > 0)
            {
                Reload();
            }
            else
            {
                if (fire)
                {
                    Fire(seed);
                }
            }
        }
    }

    protected virtual void Fire(int seed)
    {
        if (CurrentAmmo >= weaponStat.ammoPerShot)
        {
            if (fireFrame + fireInterval <= BoltNetwork.ServerFrame)
            {
                int dmg = 0;
                fireFrame = BoltNetwork.ServerFrame;

                if (playerCallback.entity.IsOwner)
                    playerCallback.FireEffect(weaponStat.precision, seed);

                if (playerCallback.entity.HasControl)
                    FireEffect(seed, weaponStat.precision);

                CurrentAmmo -= weaponStat.ammoPerShot;
                UnityEngine.Random.InitState(seed);

                dmgCounter = new Dictionary<PlayerMotor, int>();
                for (int i = 0; i < weaponStat.multiShot; i++)
                {
                    Vector2 rnd = UnityEngine.Random.insideUnitSphere * WeaponStat.precision;
                    Ray r = new Ray(camera.position,
                        (camera.forward * 10f) + (camera.up * rnd.y) + (camera.right * rnd.x));
                    RaycastHit rh;

                    if (Physics.Raycast(r, out rh, weaponStat.maxRange))
                    {
                        PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                        if (target != null)
                        {
                            if (target.IsHeadshot(rh.collider))
                                dmg = (int) (weaponStat.dmg * 1.5f);
                            else
                                dmg = weaponStat.dmg;

                            if (!dmgCounter.ContainsKey(target))
                                dmgCounter.Add(target, dmg);
                            else
                                dmgCounter[target] += dmg;
                        }
                    }
                }

                foreach (PlayerMotor pm in dmgCounter.Keys)
                    pm.Life(playerMotor, -dmgCounter[pm]);
            }
        }
        else if(TotalAmmo > 0)
            Reload();
    }

    public virtual void FireEffect(int seed, float precision)
    {
        UnityEngine.Random.InitState(seed);

        for (int i = 0; i < weaponStat.multiShot; i++)
        {
            Vector2 rnd = UnityEngine.Random.insideUnitSphere * precision;
            Ray r = new Ray(camera.position, camera.forward + (camera.up * rnd.y) + (camera.right * rnd.x));
            RaycastHit rh;

            if (Physics.Raycast(r, out rh))
            {
                if (weaponStat.impact)
                    Instantiate(weaponStat.impact, rh.point, Quaternion.LookRotation(rh.normal));

                if (weaponStat.decal)
                    if (!rh.rigidbody)
                        Instantiate(weaponStat.decal, rh.point, Quaternion.LookRotation(rh.normal));

                if (weaponStat.trail)
                {
                    var trailGo = Instantiate(weaponStat.trail, muzzle.position, Quaternion.identity);
                    var trail = trailGo.GetComponent<LineRenderer>();

                    trail.SetPosition(0, muzzle.position);
                    trail.SetPosition(1, rh.point);
                }
            }
            else if (weaponStat.trail)
            {
                var trailGo = Instantiate(weaponStat.trail, muzzle.position, Quaternion.identity);
                var trail = trailGo.GetComponent<LineRenderer>();

                trail.SetPosition(0, muzzle.position);
                trail.SetPosition(1, r.direction * weaponStat.maxRange + camera.position);
            }
        }
    }

    public virtual void InitAmmo(int current, int total)
    {
        CurrentAmmo = current;
        TotalAmmo = total;
        if (playerMotor.entity.HasControl)
            GUIController.Current.UpdateAmmo(current, total);
    }

    private void Reload()
    {
        reloadCrt = StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponStat.reloadTime);
        TotalAmmo += CurrentAmmo;
        int ammo = Mathf.Min(currentTotalAmmo, weaponStat.magazin);
        TotalAmmo -= ammo;
        CurrentAmmo = ammo;
        isReloading = false;
    }
}
