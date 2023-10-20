using UnityEngine;

/// <summary>
/// Stores all the bloons and has helper function to get a new bloon based on health. Use this class attached to an
/// empty game object to store bloons. Prevents each bloon from needing its' own links to all the possible bloons
/// that it can become when it takes damage.
/// </summary>
public class BloonLookUpScript : MonoBehaviour
{

    [SerializeField] 
    private GameObject redBloonPrefab;

    [SerializeField] 
    private GameObject blueBloonPrefab;

    [SerializeField] 
    private GameObject greenBloonPrefab;

    [SerializeField] 
    private GameObject yellowBloonPrefab;

    [SerializeField] 
    private GameObject pinkBloonPrefab;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (redBloonPrefab == null || blueBloonPrefab == null || greenBloonPrefab == null || yellowBloonPrefab == null || pinkBloonPrefab == null)
        {
            Debug.LogError(name + " is missing prefab links!");
        }
    }

    /// <summary>
    /// Allows a bloon to retrieve a new bloon prefab to instantiate based on the bloons new health after taking damage.
    /// </summary>
    /// <param name="health">The amountToSpawn of health that the bloon it should spawn should have. i.e. 4 = yellow bloon etc.</param>
    /// <returns>A prefab of the new bloon to create</returns>
    public GameObject GetNewBloon(int health)
    {
        return health switch
        {
            5 => pinkBloonPrefab,
            4 => yellowBloonPrefab,
            3 => greenBloonPrefab,
            2 => blueBloonPrefab,
            1 => redBloonPrefab,
            _ => null
        };
    }
}
