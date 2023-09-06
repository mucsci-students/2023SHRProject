using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloonScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rigidbody2D;
    
    /// <summary>
    /// Speed of the bloon. It will be consistent in X/Y direction and always greater than 0, which removes the need for a Vector2 type.
    /// </summary>
    [SerializeField] private uint speed;

    [SerializeField] private int health;

    [SerializeField] private BloonLookUpScript bloonLookUpScript;

    // Start is called before the first frame update
    private void Start()
    {
        if (Rigidbody2D == null)
        {
            Debug.LogWarning("Missing direct link on " + name);
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        if (bloonLookUpScript == null)
        {
            Debug.LogError("Missing direct link on " + name);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    /// <summary>
    /// Updates the bloons health based on damage param. Destroys itself if it loses all its health
    /// </summary>
    /// <param name="damage">The amount of damage the bloon should take</param>
    /// <returns>The amount of damage the bloon actually took, to be able to update pop counts</returns>
    public int ReceiveDamage(int damage)
    {
        var originalHealth = health;
        health -= damage;
        
        if (health <= 0)
        {
            Destroy(gameObject);
            WaveManager.enemiesRemaining -= 1;
        }
        else
        {
            var newBloon = Instantiate(bloonLookUpScript.GetNewBloon(health), gameObject.transform.position, Quaternion.identity);

            var newBloonScript = newBloon.GetComponent<BloonScript>();
            newBloonScript.SetBloonLookUpScript(bloonLookUpScript);
            
            var newBloonFollowingScript = newBloon.GetComponent<PathFollowingScript>();
            var currentBloonFollowingScript = GetComponent<PathFollowingScript>();
            newBloonFollowingScript.SetBloonPath(currentBloonFollowingScript.GetBloonPath());
            newBloonFollowingScript.SetCurrentTargetIndex(currentBloonFollowingScript.GetCurrentTargetIndex());
            newBloonFollowingScript.SetDistanceTravelled(currentBloonFollowingScript.GetDistanceTravelled());
            
            Destroy(gameObject);
        }
        
        return originalHealth - Math.Max(0, health);
    }

    public uint GetSpeed()
    {
        return speed;
    }

    public void SetBloonLookUpScript(BloonLookUpScript lookUpScript)
    {
        bloonLookUpScript = lookUpScript;
    }
}
