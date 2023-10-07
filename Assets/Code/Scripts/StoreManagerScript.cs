using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MonkeySpawner : MonoBehaviour
{
    private GameObject monkeyPrefab;
    private bool isPlacingMonkey;

    private MonkeyScript currentMonkeyInUpgradesMenu;
    [SerializeField] private GameObject UpgradeMenuCanvas;
    [SerializeField] private Image MonkeyImage;
    //NOTE: ADD TARGETING MODE AS A LIST? OR SLIDER? or somethin else?
    [SerializeField] private TextMeshProUGUI monkeyNameText;
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        
        if (isPlacingMonkey)
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Tile tile))
            {
                if (!CanBuyTower())
                {
                    Debug.Log("no monkey for you");
                    isPlacingMonkey = false;
                    return;
                }
                    
                PlaceMonkeyOnTile(tile);
                return;
            }
        }
        
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out MonkeyScript monkeyScript) && !isPlacingMonkey)
        {
            // If we already have that monkey open in the upgrade menu and we clicked the monkey again, close it
            if (UpgradeMenuCanvas.activeInHierarchy && currentMonkeyInUpgradesMenu == monkeyScript)
            {
                closeUpgradeMenu();
                currentMonkeyInUpgradesMenu = null;
            }
            else
            {
                // We clicked a monkey and we are not placing a tower so show upgrade menu
                showUpgradeCanvas(monkeyScript);
                currentMonkeyInUpgradesMenu = monkeyScript;
            }
        }
    }

    public void showUpgradeCanvas(MonkeyScript currentMonkey)
    {
        UpgradeMenuCanvas.SetActive(true);
        monkeyNameText.text = currentMonkey.GetMonkeyName();
        MonkeyImage.sprite = currentMonkey.GetMonkeyImage();
        
        //add targeting mode - suggestions??? slider menu? or somthin, still thinkin 
        
        //also add sell price? ( based off round and purchased upgrades,
            //so keep track of upgrade costs player gets and add it to variable and put it into sell button
            //also make sure to update current money if selling
            
        //dont purchase in here, player might just want to view upgrades/monkey
        
        //also add how many bloons it popped, its popping power, other helpful info for player
    }

    public void closeUpgradeMenu()
    {
        UpgradeMenuCanvas.SetActive(false);
    }

    public void purchaseUpgradePath1()
    {
        //note: also make sure to decrease money when purchased
        
        if (currentMonkeyInUpgradesMenu.GetUpgradePath1().Count == 0)
            return;
        currentMonkeyInUpgradesMenu.GetUpgradePath1()[0].UpgradeTower();
    }

    public void purchaseUpgradePath2()
    {
        //note: also make sure to decrease money when purchased
        
        if (currentMonkeyInUpgradesMenu.GetUpgradePath2().Count == 0)
            return;
        currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].UpgradeTower();
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
