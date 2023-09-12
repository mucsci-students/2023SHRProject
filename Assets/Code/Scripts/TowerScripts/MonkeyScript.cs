using System;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyScript : MonoBehaviour
{

    private readonly List<GameObject> _enemiesInRange = new();

    [Header("Tower Settings")]
    
    [SerializeField] 
    [Tooltip("Measured as seconds between firing.")]
    [Min(0.01f)]
    private float firingRate = 1f;

    [SerializeField] 
    protected Enums.TargetingMode targetingMode = Enums.TargetingMode.First; 
    
    [SerializeField]
    [Range(0, 720)]
    [Tooltip("Rotation speed in angular degrees per second")]
    private float rotateSpeed = 270f;
    
    [Header("Projectile Settings")]
    
    [SerializeField]
    protected float projectileSpeed = 1f;
    [SerializeField]
    protected float maxProjectileDistance = 10f;
    [SerializeField]
    [Min(0)]
    protected uint layersPoppedPerHit = 1;
    [SerializeField]
    protected uint pierceAmount = 1;

    [Header("Projectile")] 
    
    [SerializeField]
    protected GameObject projectilePrefab;
    
    [Header("Stats")] 
    
    [SerializeField]
    private int totalLayersPopped = 0;

    [Header("Object Links")] 
    
    [SerializeField]
    private SpriteRenderer radiusSpriteRenderer;

    private float _timer = 0f;

    protected virtual void Start()
    {
        SetIsShowingRadius(false);
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_enemiesInRange.Count > 0)
        {
            var target = GetTarget(targetingMode);
            if (target == null || _enemiesInRange.Count == 0)
            {
                return;
            }
            
            LookAt(target.transform.position);
            if (_timer >= firingRate)
            {
                Fire(target);
                _timer = 0;
            }
        }
    }

    public void IncrementLayersPopped(int layersPopped)
    {
        totalLayersPopped += layersPopped;
    }

    private void LookAt(Vector3 targetPosition)
    {
        Vector3 myLocation = transform.position;
        targetPosition.z = myLocation.z; // ensure there is no 3D rotation by aligning Z position
 
        Vector3 vectorToTarget = targetPosition - myLocation;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 0) * vectorToTarget;
 
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);

        // Angular speed in degrees per sec.
        var maxRotateSpeed = rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotateSpeed);
    }

    protected virtual void Fire(GameObject target)
    { 
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
    }

    private GameObject GetTarget(Enums.TargetingMode currentTargetingMode)
    {
        return currentTargetingMode switch
        {
            Enums.TargetingMode.First => GetFirstTarget(),
            Enums.TargetingMode.Last => GetLastTarget(),
            Enums.TargetingMode.Strongest => GetStrongestTarget(),
            Enums.TargetingMode.None => GetFirstTarget(),
            _ => GetFirstTarget()
        };
    }

    private GameObject GetFirstTarget()
    {
        GameObject target = null;
        var maxDistanceTravelled = Mathf.NegativeInfinity;

        foreach (var enemy in _enemiesInRange)
        {
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                continue;
            }
            
            var distanceTravelled = enemy.GetComponent<PathFollowingScript>().GetDistanceTravelled();
            if (distanceTravelled > maxDistanceTravelled)
            {
                maxDistanceTravelled = distanceTravelled;
                target = enemy;
            }
        }
        
        return target;
    }

    private GameObject GetLastTarget()
    {
        GameObject target = null;
        var leastDistanceTravelled = Mathf.Infinity;

        foreach (var enemy in _enemiesInRange)
        {
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                continue;
            }
            
            var distanceTravelled = enemy.GetComponent<PathFollowingScript>().GetDistanceTravelled();
            if (distanceTravelled < leastDistanceTravelled)
            {
                leastDistanceTravelled = distanceTravelled;
                target = enemy;
            }
        }
        
        return target;
    }

    private GameObject GetStrongestTarget()
    {
        GameObject target = null;
        var strongestHealth = Mathf.NegativeInfinity;
        var maxDistanceTravelled = Mathf.NegativeInfinity;
        const float tolerance = 0.1f;

        foreach (var enemy in _enemiesInRange)
        {
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                continue;
            }
            
            var distanceTravelled = enemy.GetComponent<PathFollowingScript>().GetDistanceTravelled();
            var enemyHealth = enemy.GetComponent<BloonScript>().GetHealth();
            if (enemyHealth > strongestHealth)
            {
                strongestHealth = enemyHealth;
                maxDistanceTravelled = distanceTravelled;
                target = enemy;
            } else if (Math.Abs(strongestHealth - enemyHealth) < tolerance && distanceTravelled > maxDistanceTravelled)
            {
                strongestHealth = enemyHealth;
                maxDistanceTravelled = distanceTravelled;
                target = enemy;
            }
        }
        
        return target;
    }

    private void OnMouseDown()
    {
        ToggleIsShowingRadius();
    }
    
    public void AddBloonToRange(GameObject bloon)
    {
        _enemiesInRange.Add(bloon);
    }
    
    public void RemoveBloonFromRange(GameObject bloon)
    {
        _enemiesInRange.Remove(bloon);
    }

    public void ToggleIsShowingRadius()
    {
        SetIsShowingRadius(!radiusSpriteRenderer.enabled);
    }

    public void SetIsShowingRadius(bool state)
    {
        radiusSpriteRenderer.enabled = state;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _enemiesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _enemiesInRange.Remove(other.gameObject);
    }
}