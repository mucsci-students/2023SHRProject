using System.Collections.Generic;
using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    private readonly List<GameObject> _enemiesInRange = new ();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            _enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            _enemiesInRange.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetEnemiesInRange()
    {
        return _enemiesInRange;
    }
}
