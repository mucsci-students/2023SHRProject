using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    private List<GameObject> enemiesInRange = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetEnemiesInRange()
    {
        return enemiesInRange;
    }
}
