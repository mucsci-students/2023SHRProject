using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloonLookUpScript : MonoBehaviour
{

    [SerializeField] 
    private GameObject RedBloonPrefab;

    [SerializeField] 
    private GameObject BlueBloonPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        if (RedBloonPrefab == null || BlueBloonPrefab == null)
        {
            Debug.LogError(name + " is missing prefab links!");
        }
    }

    public GameObject GetNewBloon(int health)
    {
        if (health == 2)
        {
            return BlueBloonPrefab;
        } else if (health == 1)
        {
            return RedBloonPrefab;
        }

        return null;
    }
}
