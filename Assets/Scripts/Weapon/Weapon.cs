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

    protected int fireFrame = 0;
    private Coroutine reloadCrt = null;
    protected Dictionary<PlayerMotor, int> dmgCounter;

    protected int fireInterval
    {
        get
        {
            int rps = weaponStat.rpm / 60;
            return BoltNetwork.FramesPerSecond / rps;
        }
    }

    public WeaponStats WeaponStat => weaponStat;
    public int CurrentAmmo => currentAmmo;
    public int TotalAmmo => currentTotalAmmo;

    public virtual void Init(PlayerWeapons pw)
    {
        playerWeapons = pw;
        playerMotor = pw.GetComponent<PlayerMotor>();
        camera = playerWeapons.Cam.transform;

        currentAmmo = weaponStat.magazin;
        currentTotalAmmo = weaponStat.totalMagazin;
    }

    public virtual void ExecuteCommand(bool fire, bool aiming, bool reload, int seed)
    {
        if (!isReloading)
        {
            if (reload && currentAmmo != weaponStat.magazin && currentTotalAmmo > 0)
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
        if (currentAmmo >= weaponStat.ammoPerShot)
        {
            if (fireFrame + fireInterval <= BoltNetwork.ServerFrame)
            {
                int dmg = 0;
                fireFrame = BoltNetwork.ServerFrame;
                //Todo fire effect

                currentAmmo -= weaponStat.ammoPerShot;
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
        else if(currentTotalAmmo > 0)
            Reload();
    }

    private void Reload()
    {
        reloadCrt = StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponStat.reloadTime);
        currentTotalAmmo += currentAmmo;
        int ammo = Mathf.Min(currentTotalAmmo, weaponStat.magazin);
        currentTotalAmmo -= ammo;
        currentAmmo = ammo;
        isReloading = false;
    }
}
