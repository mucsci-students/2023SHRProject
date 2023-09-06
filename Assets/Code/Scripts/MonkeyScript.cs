using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MonkeyScript : MonoBehaviour
{

    private readonly List<GameObject> _enemiesInRange = new();

    [Header("Tower Settings")]
    
    [SerializeField] 
    private float firingRate = 1f;

    [SerializeField] 
    private Enums.TargetingMode targetingMode = Enums.TargetingMode.First; 
    
    [Header("Projectile Settings")]
    
    [SerializeField]
    private float projectileSpeed = 1f;
    [SerializeField]
    private float maxProjectileDistance = 10f;
    [SerializeField]
    private uint layersPoppedPerHit = 1;
    [SerializeField]
    private uint pierceAmount = 1;

    [Header("Projectile")] 
    
    [SerializeField]
    private GameObject projectilePrefab;
    
    [Header("Stats")] 
    
    [SerializeField]
    private int totalLayersPopped = 0;

    [Header("Object Links")] 
    
    [SerializeField]
    private GameObject radius;

    private float _timer = 0f;


    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_enemiesInRange.Count > 0)
        {
            var target = GetTarget(targetingMode);
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
        var maxRotateSpeed = 270f * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotateSpeed);
    }

    private void Fire(GameObject target)
    { 
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
    }

    private GameObject GetTarget(Enums.TargetingMode targetingMode)
    {
        switch (targetingMode)
        {
            case Enums.TargetingMode.First:
                return GetFirstTarget();
            case Enums.TargetingMode.Last:
                return GetLastTarget();
            case Enums.TargetingMode.Strongest:
                return GetStrongestTarget();
            default:
                return GetFirstTarget();
        }
    }

    private GameObject GetFirstTarget()
    {
        GameObject target = null;
        var maxDistanceTravelled = -1f;

        foreach (var enemy in _enemiesInRange)
        {
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
        return null;
    }

    private GameObject GetStrongestTarget()
    {
        return null;
    }

    public void ToggleIsShowingRadius()
    {
        SetIsShowingRadius(!radius.activeInHierarchy);
    }

    public void SetIsShowingRadius(bool state)
    {
        radius.SetActive(state);
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
