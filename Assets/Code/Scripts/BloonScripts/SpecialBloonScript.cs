using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBloonScript : BloonScript
{
    [SerializeField] private List<GameObject> bloonsToSpawn = new();
    [SerializeField] private float SecondsBetweenBloonSpawns = 0.1f;

    /// <summary>
    /// Updates the bloons health based on damage param. Destroys itself if it loses all its health
    /// </summary>
    /// <param name="damage">The amountToSpawn of damage the bloon should take</param>
    /// <returns>The amountToSpawn of damage the bloon actually took, to be able to update pop counts</returns>
    public override int ReceiveDamage(int damage)
    {
        var originalHealth = health;
        health -= damage;
        
        if (!gameObject.activeInHierarchy)
            return 0;
        
        if (health <= 0)
        {
            Destroy(gameObject);
            WaveManager.enemiesRemaining -= 1;
        }
        else
        {
            var coroutine = SpawnBloons();

            //Disable the rigidbody
            Rigidbody2D.simulated = false;
            //Disable the collider
            GetComponent<Collider2D>().enabled = false;
            //Disable the sprite renderer
            GetComponent<SpriteRenderer>().enabled = false;
            
            WaveManager.enemiesRemaining -= 1;
            WaveManager.enemiesRemaining += bloonsToSpawn.Count;

            StartCoroutine(coroutine);
        }

        GameManager.Money += originalHealth - Math.Max(0, health);
        return originalHealth - Math.Max(0, health);
    }
    
    private IEnumerator SpawnBloons()
    {
        foreach (var bloon in bloonsToSpawn)
        {
            SpawnAndInitializeBloon(bloon);
            yield return new WaitForSeconds(SecondsBetweenBloonSpawns);
        }
        
        Destroy(gameObject);
    }
}
