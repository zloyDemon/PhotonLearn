using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    private static float SpeedMultiplier = 1.5f;
    private static int DamageMultiplier = 2;
    private static float BackAngleThreshold = 60f;

    private void OnEnable()
    {
        if (playerMotor)
        {
            playerMotor.Speed = playerMotor.SpeedBase * SpeedMultiplier;
            GUIController.Current.HideAmmo();
        }
    }

    private void OnDisable()
    {
        playerMotor.Speed = playerMotor.SpeedBase;
    }

    protected override void Fire(int seed)
    {
        if (fireFrame + fireInterval <= BoltNetwork.ServerFrame)
        {
            fireFrame = BoltNetwork.ServerFrame;
            if(playerCallback.entity.IsOwner)
                playerCallback.FireEffect(seed, 0);
            FireEffect(seed, 0);

            Ray r = new Ray(camera.position, camera.forward);
            RaycastHit rh;

            if (Physics.Raycast(r, out rh, weaponStat.maxRange))
            {
                PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                if (target != null)
                {
                    int dmg = weaponStat.dmg;
                    if (Vector3.Angle(Vector3.Scale(camera.forward, new Vector3(1, 0, 1)).normalized,
                        target.transform.forward) < BackAngleThreshold)
                        dmg *= DamageMultiplier;
                    target.Life(playerMotor, - dmg);
                }
            }
        }
    }
}
