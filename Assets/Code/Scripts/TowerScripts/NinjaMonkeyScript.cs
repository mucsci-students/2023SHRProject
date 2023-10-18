using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaMonkeyScript : MonkeyScript
{
    private int numberOfProjectiles;


    #region No Distribution For Projectiles Code
    // protected override void Fire(GameObject target)
    // {
    //     var originalProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    //     var originalProjectileScript = originalProjectile.GetComponent<ProjectileScript>();
    //     originalProjectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
    //     if (numberOfProjectiles >= 2)
    //     {
    //         float spaceBetweenProjectiles = 0.2f;
    //         for (int i = 0; i < numberOfProjectiles; i++)
    //         {
    //             // Calculate the direction for the new projectile
    //             Vector3 direction = Quaternion.Euler(0, 0, (i / (float)numberOfProjectiles) * 30f) * originalProjectile.transform.up;
    //
    //             // Calculate the position for the new projectile based on the direction and space
    //             Vector3 offsetPosition = direction * spaceBetweenProjectiles * (i + 1);
    //
    //             // Create the additional projectile
    //             var projectile = Instantiate(projectilePrefab, transform.position + offsetPosition, Quaternion.identity);
    //             var projectileScript = projectile.GetComponent<ProjectileScript>();
    //             projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
    //         }
    //     }
    // }
    #endregion

    protected override void Fire(GameObject target)
    {
        //this is for the original projectile - basically only 1 projectile
        var originalProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var originalProjectileScript = originalProjectile.GetComponent<ProjectileScript>();
        originalProjectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
        
        //they bought the upgrade to throw more shurikens
        if (numberOfProjectiles >= 3)
        {
            float spaceBetweenProjectiles = 0.2f;
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Vector3 direction;
                //these help to distribute the projectiles in a pattern
                if (i % 2 == 0)
                {
                    direction = Quaternion.Euler(0, 0, i / 2 * 30f) * originalProjectile.transform.up;
                }
                else
                {
                    direction = Quaternion.Euler(0, 0, -(i / 2 * 30f)) * originalProjectile.transform.up;
                }

                // Calculate the position for the new projectile based on the direction and space
                Vector3 offsetPosition = direction * spaceBetweenProjectiles * (i / 2 + 1);

                // Create the additional projectile
                var projectile = Instantiate(projectilePrefab, transform.position + offsetPosition, Quaternion.identity);
                var projectileScript = projectile.GetComponent<ProjectileScript>();
                projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
            }
        }
    }
    
    protected override void Upgrade1_1()
    {
        // Increases attack range and attack speed.
        radiusSpriteRenderer.transform.localScale *= 1.2f;
        projectileSpeed = 20;
    }

    protected override void Upgrade1_2()
    {
        //Shurikens can pop 4 bloons each.
        layersPoppedPerHit = 4;
    }

    protected override void Upgrade2_1()
    {
        //Extreme Ninja skill enables him to throw 3 shurikens at once.
        numberOfProjectiles = 3;
    }

    protected override void Upgrade2_2()
    {
       //The art of bloonjitsu allows Ninjas to throw 5 deadly shurikens at once!
       numberOfProjectiles = 5;
    }
}