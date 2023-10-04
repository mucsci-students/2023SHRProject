using UnityEngine;

/// <summary>
/// Handles all logic for the tack tower.
/// </summary>
public class TackTowerScript : MonkeyScript
{
    
    private readonly Vector3[] directions = 
    {
        new Vector3(0f, 1f),
        new Vector3(1f, 0f),
        new Vector3(0f, -1f),
        new Vector3(-1f, 0f),
        new Vector3(1f, 1f),
        new Vector3(1f, -1f),
        new Vector3(-1f, -1f),
        new Vector3(-1f, 1f),
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
        foreach (Vector3 direction in directions)
        {
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            var projectileScript = projectile.GetComponent<ProjectileScript>();
            projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, direction, this);
        }
    }

    protected override void Upgrade1_1()
    {
        
    }
    
    protected override void Upgrade1_2()
    {
        
    }
    
    protected override void Upgrade2_1()
    {
        
    }
    
    protected override void Upgrade2_2()
    {
        
    }
    
}
