using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class MonkeyScript : MonoBehaviour
{

    private readonly List<GameObject> _enemiesInRange = new();

    [Header("Tower Settings")]
    
    [SerializeField] 
    private float firingRate = 1f;
    
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
    private int layersPopped = 0;

    private float _timer = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_enemiesInRange.Count > 0)
        {
            LookAt(_enemiesInRange[0].transform.position);
            if (_timer >= firingRate)
            {
                Fire(_enemiesInRange[0]);
                _timer = 0;
            }
        }
    }

    public void IncrementLayersPopped(int LayersPopped)
    {
        layersPopped += LayersPopped;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        _enemiesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _enemiesInRange.Remove(other.gameObject);
    }
}
