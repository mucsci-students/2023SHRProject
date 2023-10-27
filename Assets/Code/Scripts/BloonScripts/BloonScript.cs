using System;
using UnityEngine;

public class BloonScript : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D Rigidbody2D;
    
    /// <summary>
    /// Speed of the bloon. It will be consistent in X/Y direction and always greater than 0, which removes the need for a Vector2 type.
    /// </summary>
    [SerializeField] protected uint speed;

    [SerializeField] protected int health;

    [SerializeField] protected BloonLookUpScript bloonLookUpScript;

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

    /// <summary>
    /// Updates the bloons health based on damage param. Destroys itself if it loses all its health
    /// </summary>
    /// <param name="damage">The amountToSpawn of damage the bloon should take</param>
    /// <returns>The amountToSpawn of damage the bloon actually took, to be able to update pop counts</returns>
    public virtual int ReceiveDamage(int damage)
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
            SpawnAndInitializeBloon(bloonLookUpScript.GetNewBloon(health));
            
            Destroy(gameObject);
        }
        
        gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GameManager.Money += originalHealth - Math.Max(0, health);
        return originalHealth - Math.Max(0, health);
    }

    protected void SpawnAndInitializeBloon(GameObject bloonPrefab)
    {
        var newBloon = Instantiate(bloonPrefab, gameObject.transform.position, Quaternion.identity);

        var newBloonScript = newBloon.GetComponent<BloonScript>();
        newBloonScript.SetBloonLookUpScript(bloonLookUpScript);
            
        var newBloonFollowingScript = newBloon.GetComponent<PathFollowingScript>();
        var currentBloonFollowingScript = GetComponent<PathFollowingScript>();
        newBloonFollowingScript.SetBloonPath(currentBloonFollowingScript.GetBloonPath());
        newBloonFollowingScript.SetCurrentTargetIndex(currentBloonFollowingScript.GetCurrentTargetIndex());
        newBloonFollowingScript.SetDistanceTravelled(currentBloonFollowingScript.GetDistanceTravelled());
    }

    public uint GetSpeed()
    {
        return speed;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetBloonLookUpScript(BloonLookUpScript lookUpScript)
    {
        bloonLookUpScript = lookUpScript;
    }
}
