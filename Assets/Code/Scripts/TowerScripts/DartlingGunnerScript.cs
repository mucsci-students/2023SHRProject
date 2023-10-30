using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartlingGunnerScript : MonkeyScript
{
    // Number of projectiles to shoot in each burst
    [SerializeField]
    private int projectilesPerBurst = 3;

    // Time delay between each projectile in a burst
    [SerializeField]
    private float burstDelay = 0.2f;

    // Spread angle in degrees
    [SerializeField]
    private float spreadAngle = 10.0f;

    protected override void Fire(GameObject target)
    {
        StartCoroutine(ShootBurst(target));
    }

    private IEnumerator ShootBurst(GameObject target)
    {
        // Get the mouse position in the world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the same z position as the tower

        for (int i = 0; i < projectilesPerBurst; i++)
        {
            // Calculate the direction towards the mouse position with spread
            Vector3 direction = Quaternion.Euler(0, 0, spreadAngle * (i - (projectilesPerBurst - 1) / 2.0f)) * (mousePosition - transform.position);

            // Calculate the rotation angle from the direction vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a projectile and set its attributes based on the direction and rotation
            var originalProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
            originalProjectile.transform.parent = projectileContainer;
            var originalProjectileScript = originalProjectile.GetComponent<ProjectileScript>();
            originalProjectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, direction, this);

            yield return new WaitForSeconds(burstDelay);
        }
    }

    protected override void Upgrade1_1()
    {
        //Focused Firing COST: $255
        //Description: Increases accuracy of the Dartling Gun.
        //Effect: Decreases angular deviation from 23° to 9.6°,
        //reducing projectile deviation by 60%. Also increases
        //projectile lifespan for all Buckshot projectiles by +40%.
        throw new System.NotImplementedException();
    }

    protected override void Upgrade1_2()
    {
        //Powerful Darts COST: $1,020
        //Description: Darts move faster and can pop 3 bloons each.
        //Effect: Darts gain +2 pierce per shot, pop Frozen, and increases
        //projectile speed and projectile size by 50%. For Hydra Rocket Pods,
        //gains +2 pierce and one more explosion plus extra blast radius, most other
        //Tier 3 crosspaths and above receive +50% pierce overall.
        throw new System.NotImplementedException();
    }

    protected override void Upgrade2_1()
    {
        //Faster Barrel Spin COST: $810
        //Description: Makes gun fire much faster.
        //Effect: Attacks +50% faster (0.66x attack cooldown).
        throw new System.NotImplementedException();
    }

    protected override void Upgrade2_2()
    {
        //Hydra Rocket Pods COST: $4,080
        //Description: Shoots depleted bloontonium missiles that can damage all
        //Bloon types and trigger multiple explosions.
        //Effect: Hydra Rocket Pod missiles damage any bloon type and produce up to
        //3 low-pierce explosions by default. Gains even more explosions with pierce buffs.
        throw new System.NotImplementedException();
    }
}