using UnityEngine;

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
