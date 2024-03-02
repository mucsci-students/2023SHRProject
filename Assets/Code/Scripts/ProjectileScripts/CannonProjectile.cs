using System.Collections;
using UnityEngine;

public class CannonProjectile : ProjectileScript
{
    private ExplosionProjectile _explosionProjectile;

    protected override IEnumerator Start()
    {
        yield return base.Start();
        _explosionProjectile = transform.GetChild(0).GetComponent <ExplosionProjectile>();
    }

    private void KillAllInRange()
    {
        var enemies = _explosionProjectile.GetEnemiesInRange();
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
