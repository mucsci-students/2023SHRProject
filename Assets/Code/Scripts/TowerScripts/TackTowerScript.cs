using UnityEngine;

/// <summary>
/// Handles all logic for the tack tower.
/// </summary>
public class TackTowerScript : MonkeyScript
{
    
    private readonly Vector3[] _directions = 
    {
        new(0f, 1f),
        new(1f, 0f),
        new(0f, -1f),
        new(-1f, 0f),
        new(1f, 1f),
        new(1f, -1f),
        new(-1f, -1f),
        new(-1f, 1f),
    };

    
    protected override void Start()
    {
        base.Start();
        targetingMode = Enums.TargetingMode.None;
    }

    /// <summary>
    /// Fires 8 projectiles in all directions
    /// </summary>
    /// <param name="target">Unused</param>
    protected override void Fire(GameObject target)
    {
        foreach (Vector3 direction in _directions)
        {
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.transform.parent = projectileContainer;
            var projectileScript = projectile.GetComponent<ProjectileScript>();
            projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, direction, this);
        }
    }

    protected override void Upgrade1_1()
    {
        //Faster Shooting 
        firingRate = 1.25f;
    }
    
    protected override void Upgrade1_2()
    {
        //Even Faster Shooting
        firingRate = 1f;
    }
    
    protected override void Upgrade2_1()
    {
        //Extra Range Tacks
        radiusSpriteRenderer.transform.localScale *= 1.1f;
    }
    
    protected override void Upgrade2_2()
    {
        //Super Range Tacks
        radiusSpriteRenderer.transform.localScale *= 1.12f;
    }
    
}
