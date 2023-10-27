using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement and collision of projectiles.
/// </summary>
public class BoomerangProjectile : ProjectileScript
{
    private Vector3 midpoint = Vector3.zero;
    public bool isReturningToTower = false;

    public bool invincible = false;
    public float noDamageTimeLimit = 0.00000001f;
    public float timer = 0f;
    public float alpha = 1f;

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
            
            if (invincible)
            {
                timer += Time.deltaTime;
                if (timer >= noDamageTimeLimit)
                {
                    invincible = false;
                    timer = 0f;
                }
            }
        }
    }

    private void Circle ()
    {
        if (!isReturningToTower)
        {
            midpoint = new Vector3((transform.position.x + _parentMonkeyScript.gameObject.transform.position.x) / 2f, (transform.position.y + _parentMonkeyScript.gameObject.transform.position.y) / 2f);
            isReturningToTower = true;
        } 
        float degreesPerSecond = 2500f; // Desired rotation speed in degrees per second
        float radius = Vector3.Distance(transform.position, midpoint);
        float circumference = 2 * Mathf.PI * radius;
        float anglePerFrame = (degreesPerSecond / 360f) * 360f * (Time.deltaTime / circumference);
        //transform.position = new Vector2(midpoint.x + (1.5f * Mathf.Sin(Mathf.Deg2Rad * alpha)), midpoint.y + ( 2f * Mathf.Cos(Mathf.Deg2Rad * alpha)));
        transform.RotateAround(midpoint, Vector3.forward, anglePerFrame);
        //alpha += 0.5f;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && !invincible && other.gameObject.CompareTag("Bloon"))
        {
            var bloonScript = other.gameObject.GetComponent<BloonScript>();
            // Objects get destroyed at the end of the frame. This prevents bloon from being destroyed twice by two bloons.
            if (bloonScript.GetHealth() <= 0)
            {
                return;
            }

            var layersPopped = bloonScript.ReceiveDamage((int)_layersPerHit);
            _parentMonkeyScript.IncrementLayersPopped(layersPopped);

            invincible = true;
            _target = null;

        }
        else if (isReturningToTower && other.gameObject == _parentMonkeyScript.gameObject)
        {
            Destroy(gameObject);
            Destroy(this);
            _layersPerHit = 0;
        }
    }
}


