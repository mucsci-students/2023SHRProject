using UnityEngine;
using UnityEngine.UI;

public class MonkeySpawner : MonoBehaviour
{
    private GameObject monkeyPrefab;
    private bool isPlacingMonkey = false;

    private void Update()
    {
        if (isPlacingMonkey)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
 
                if (hit.collider != null && hit.collider.gameObject.TryGetComponent<Tile>(out Tile tile))
                {
                    if (tile.ContainsTowers())
                    {
                        return;
                    }
                    
                    Vector3 tilePosition = tile.transform.position;
                    tilePosition.z = 0;
                    
                    Instantiate(monkeyPrefab, tilePosition, Quaternion.identity);
                    isPlacingMonkey = false; //no more monkey for you!
                    
                    tile.SetContainsTower(true);
                }
            }
        }
    }
    
    public void SetMonkeyPrefab(GameObject newMonkeyPrefab)
    {
        monkeyPrefab = newMonkeyPrefab;
    }
    
    public void StartPlacingMonkey() //you can start placin some monkey -- replace with money mechanic!!
    {
        isPlacingMonkey = true;
    }
}



//grid - [10]
//monkey - [0]
//monkey radius - [1]
//monkey projectiles/objects?
//bloons [2 - 3]
    //blue bloon - [2.85]
    //red bloon - [2.9]
//road & path - [3]

//UI - drawn over
