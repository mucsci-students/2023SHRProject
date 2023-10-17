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
    //Note: add targeting mode thing 
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
        
        //add targeting mode
        
        //add sell price: based off purchase price and upgrades
            //keep track of upgrades player gets and add it to var and put it into sell var
            //also update current money if selling
        
        //also add how many bloons it popped, its popping power, other helpful info for player
    }

    public void closeUpgradeMenu()
    {
        //I removed the button from the menu so this is not needed
        UpgradeMenuCanvas.SetActive(false);
    }

    public void purchaseUpgradePath1()
    {
        if (currentMonkeyInUpgradesMenu.GetUpgradePath1().Count == 0)
            return;
        
        currentMonkeyInUpgradesMenu.GetUpgradePath1()[0].UpgradeTower();
        //GameManager.Money -= currentMonkeyInUpgradesMenu.GetUpgradePath1()[0].GetCost();
        //Debug.Log(currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].GetCost());
    }

    public void purchaseUpgradePath2()
    {
        if (currentMonkeyInUpgradesMenu.GetUpgradePath2().Count == 0)
            return;
        currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].UpgradeTower();
        //GameManager.Money -= currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].GetCost();
    }
    
    public void SetMonkeyPrefab(GameObject newMonkeyPrefab)
    {
        monkeyPrefab = newMonkeyPrefab;
    }
    
    public void StartPlacingMonkey()
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
