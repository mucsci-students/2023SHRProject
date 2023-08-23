using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyScript : MonoBehaviour
{

    private readonly List<GameObject> _enemiesInRange = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (_enemiesInRange.Count > 0)
        {
            LookAt(_enemiesInRange[0].transform.position);
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        _enemiesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _enemiesInRange.Remove(other.gameObject);
    }
}
