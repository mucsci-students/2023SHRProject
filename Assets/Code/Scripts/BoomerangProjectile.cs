using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles the movement and collision of projectiles.
/// </summary>
public class BoomerangProjectile : ProjectileScript
{
    private Vector3 midpoint = Vector3.zero; 
    protected override void Update()
    {
        
        if (_distanceTraveled > _maxDistance)
        {
            Destroy(gameObject);
        }
        else if (_target != null)
        {
            LookAt(_target.transform.position);
            MoveTowardsTarget(_target.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            Circle();
        }
    }

    private void Circle ()
    {
        
        if (midpoint.x == 0f && midpoint.y == 0f)
        {
            midpoint = new Vector3((transform.position.x + _parentMonkeyScript.gameObject.transform.position.x) / 2f, (transform.position.y + _parentMonkeyScript.gameObject.transform.position.y) / 2f);
            Debug.Log(midpoint);
        } 
        transform.RotateAround(midpoint, Vector3.forward, 180f * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject);
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

            
        }
        else if (other.gameObject == _parentMonkeyScript.gameObject)
        {
            Destroy(gameObject);
            Destroy(this);
            _layersPerHit = 0;
        }
    }
}


