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
                    if (!CanBuyTower())
                    {
                        Debug.Log("no monkey for you");
                        isPlacingMonkey = false;
                        return;
                    }
                        
                    PlaceMonkeyOnTile(tile);
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

    public bool CanBuyTower()
    {
        return GameManager.Money >= monkeyPrefab.GetComponent<MonkeyScript>().GetMonkeyCost();
    }
    
    public bool PlaceMonkeyOnTile(Tile tile)
    {
        if (tile.ContainsTowers())
            return false;
        
        Vector3 tilePosition = tile.transform.position;
        tilePosition.z = 0;
                    
        Instantiate(monkeyPrefab, tilePosition, Quaternion.identity);
        isPlacingMonkey = false; //no more monkey for you!
                    
        tile.SetContainsTower(true);

        GameManager.Money -= monkeyPrefab.GetComponent<MonkeyScript>().GetMonkeyCost();

        return true;
    }
}
