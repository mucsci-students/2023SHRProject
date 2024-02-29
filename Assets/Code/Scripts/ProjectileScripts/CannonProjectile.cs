using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : ProjectileScript
{
    private ExplosionProjectile explosionProjectile;

    protected override IEnumerator Start()
    {
        yield return base.Start();
        explosionProjectile = transform.GetChild(0).GetComponent <ExplosionProjectile>();
    }

    private void KillAllInRange()
    {
        var enemies = explosionProjectile.GetEnemiesInRange();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
              var layersPopped = enemies[i].GetComponent<BloonScript>().ReceiveDamage((int)LayersPerHit);
              parentMonkeyScript.IncrementLayersPopped(layersPopped);
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            var bloonScript = other.gameObject.GetComponent<BloonScript>();

            // Objects get destroyed at the end of the frame. This prevents bloon from being destroyed twice by two bloons.
            if (bloonScript.GetHealth() <= 0)
            {
                return;
            }

            KillAllInRange();

            var layersPopped = bloonScript.ReceiveDamage((int)LayersPerHit);
            parentMonkeyScript.IncrementLayersPopped(layersPopped);

            Destroy(gameObject);
            Destroy(this);
            LayersPerHit = 0;
        }
    }
}
