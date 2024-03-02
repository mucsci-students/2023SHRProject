using System.Collections;
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
    
    //  protected override void Update()
    //  { 
    //      
    //  }
    //
    
     protected override void LookAt(Vector3 targetPosition)
     {
         Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y); // Convert to Vector2
         targetPosition = mousePosition2D;
         base.LookAt(targetPosition);
     }
    
    protected override void Fire(GameObject target)
    {
        StartCoroutine(ShootBurst(target));
    }

    private IEnumerator ShootBurst(GameObject target)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y); // Convert to Vector2

        for (int i = 0; i < projectilesPerBurst; i++)
        {
            // Calculate the direction towards the mouse position with spread
            Vector3 direction = Quaternion.Euler(0, 0, spreadAngle * (i - (projectilesPerBurst - 1) / 2.0f)) * (mousePosition2D - (Vector2)transform.position);

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
        //Effect: Decreases angular deviation
        spreadAngle = 6.5f;
    }

    protected override void Upgrade1_2()
    {
        //Powerful Darts COST: $510
        //Description: Darts move faster and can pop 3 bloons each.
        projectileSpeed = 28;
        layersPoppedPerHit = 3;
    }

    protected override void Upgrade2_1()
    {
        //Faster Barrel Spin COST: $215
        //Description: Makes gun fire much faster.
        //Effect: Attacks +?% faster
        firingRate = 0.45f;
    }

    protected override void Upgrade2_2()
    {
        //Buckshot COST: $1020
        //Description: Shoots blasts of deadly buckshot instead of darts.
        //more darts, more accuracy, more speed
        //change projectile prefab??
        projectilesPerBurst = 4;
        burstDelay = 0.01f;
        firingRate = 0.6f;
    }
}
