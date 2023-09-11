using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private float _speed = 1f;
    private float _maxDistance = 10f;
    private uint _layersPerHit = 1;
    
    // TODO - Implement this 
    private uint _pierceAmount = 1;

    private float _distanceTraveled;
    private Vector3 _lastTargetDirection = Vector3.zero;

    private GameObject _target;
    private MonkeyScript _parentMonkeyScript;

    // Update is called once per frame
    private void Update()
    {
        if (_distanceTraveled > _maxDistance)
        {
            Destroy(gameObject);
        }
        else if (_target != null)
        {
            MoveTowardsTarget(_target.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            MoveInDirection(_lastTargetDirection);
        }
    }

    public void SetAllAttributes(float speed, float maxDistance, uint layersPerHit, uint pierceAmount, GameObject target, MonkeyScript parent)
    {
        _speed = speed;
        _maxDistance = maxDistance;
        _layersPerHit = layersPerHit;
        _pierceAmount = pierceAmount;
        _target = target;
        _parentMonkeyScript = parent;
    }
    
    public void SetAllAttributes(float speed, float maxDistance, uint layersPerHit, uint pierceAmount, Vector3 direction, MonkeyScript parent)
    {
        _speed = speed;
        _maxDistance = maxDistance;
        _layersPerHit = layersPerHit;
        _pierceAmount = pierceAmount;
        _lastTargetDirection = direction;
        _parentMonkeyScript = parent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            var bloonScript = other.gameObject.GetComponent<BloonScript>();
            
            // Objects get destroyed at the end of the frame. This prevents bloon from being destroyed twice by two bloons.
            if (bloonScript.GetHealth() <= 0)
            {
                return;
            }
            
            var layersPopped = bloonScript.ReceiveDamage((int)_layersPerHit);
            _parentMonkeyScript.IncrementLayersPopped(layersPopped);
            
            Destroy(gameObject);
            Destroy(this);
            _layersPerHit = 0;
        }
    }

    /// <summary>
    /// Moves the projectile towards a targetPosition based on a speed.
    /// </summary>
    /// <param name="targetPosition">The position to move this projectile towards</param>
    /// <param name="speed">Speed of the projectile. Should be calculated based on delta time.</param>
    private void MoveTowardsTarget(Vector3 targetPosition, float speed)
    {
        Vector3 newPosition = GetNewPosition(targetPosition, speed);
        _lastTargetDirection = newPosition - transform.position;
        SetPosition(newPosition);
    }

    private void MoveInDirection(Vector3 direction)
    {
        Vector3 newPosition = GetNewPosition(transform.position + direction, _speed * Time.deltaTime);
        SetPosition(newPosition);
    }

    /// <summary>
    /// Sets the position of this transform and updates distance traveled.
    /// </summary>
    /// <param name="newPosition">The new position to move the projectile to during this frame</param>
    private void SetPosition(Vector3 newPosition)
    {
        _distanceTraveled += Vector3.Distance(newPosition , transform.position);
        transform.position = newPosition;
    }

    /// <summary>
    /// Returns a new vector that will be the new position of this based on the targetPosition position and speed which limits
    /// how far the projectile will move per frame.
    /// </summary>
    /// <param name="targetPosition">The targetPosition position for this projectile to move towards</param>
    /// <param name="speed">How fast to move towards the targetPosition position</param>
    /// <returns>The new position of this projectile</returns>
    private Vector3 GetNewPosition(Vector3 targetPosition, float speed)
    {
        return Vector3.MoveTowards(transform.position, targetPosition, speed);
    }
}
