using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI UpgradePath1Text;
    [SerializeField] private TextMeshProUGUI UpgradePath2Text;
    [SerializeField] private TextMeshProUGUI SellPriceText;
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
        
        //NOTE: come back and update monkey Image? possibly?
        //NOTE: also add indicator for upgrade path 1 and 2
        
        monkeyNameText.text = currentMonkey.GetMonkeyName();
        MonkeyImage.sprite = currentMonkey.GetMonkeyImage();
        
        //if i click on a different monkey, the upgrade paths will be different so you need to check 
        //if the upgrade paths are empty or not
        //probably a better way to do this considering that this is repeated code
        if (currentMonkey.GetUpgradePath1().Count != 0)
            UpgradePath1Text.text = currentMonkey.GetUpgradePath1()[0].GetDescription() + " Cost: " + currentMonkey.GetUpgradePath1()[0].GetCost();
        else
            UpgradePath1Text.text = "No more upgrades";

        if (currentMonkey.GetUpgradePath2().Count != 0)
            UpgradePath2Text.text = currentMonkey.GetUpgradePath2()[0].GetDescription() + " Cost: " + currentMonkey.GetUpgradePath2()[0].GetCost();
        else
            UpgradePath2Text.text = "No more upgrades";
        
        SellPriceText.text = "Sell Price: " + currentMonkey.GetMonkeySellPrice();
        //check if this runs 2x later

        //add targeting mode thing
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
        List<Upgrade> upgradePath1 = currentMonkeyInUpgradesMenu.GetUpgradePath1();
        
        if (upgradePath1.Count == 0 || !CanBuyUpgradePath1())
            return;

        Upgrade CurrentUpgrade = upgradePath1[0];
        
        //if there are 2 upgrades then we update the text to be the next upgrade (which is spot [1] in the list)
        //then we decrease money and upgrade the tower like normal
        //however, if there is only 1 upgrade left, we cant update the text to be spot [1] in the list cuz that doesnt exist
        //so insstead we can just say "no more upgrades" and then upgrade the tower like normal
        if (upgradePath1.Count > 1)
        {
            UpgradePath1Text.text = upgradePath1[1].GetDescription() + " Cost: " + upgradePath1[1].GetCost();
        }
        else
        {
            UpgradePath1Text.text = "No more upgrades";
        }

        GameManager.Money -= CurrentUpgrade.GetCost();
        currentMonkeyInUpgradesMenu.SetMonkeySellPrice(currentMonkeyInUpgradesMenu.GetMonkeySellPrice() + CurrentUpgrade.GetCost());
        SellPriceText.text = "Sell Price: " + currentMonkeyInUpgradesMenu.GetMonkeySellPrice();
        CurrentUpgrade.UpgradeTower();
    }

    public void purchaseUpgradePath2()
    {
        List<Upgrade> upgradePath2 = currentMonkeyInUpgradesMenu.GetUpgradePath2();
        
        if (upgradePath2.Count == 0 || !CanBuyUpgradePath2())
            return;
        
        Upgrade CurrentUpgrade = upgradePath2[0];
        
        if (upgradePath2.Count > 1)
        {
            UpgradePath2Text.text = upgradePath2[1].GetDescription() + " Cost: " + upgradePath2[1].GetCost();
        }
        else
        {
            UpgradePath2Text.text = "No more upgrades";
        }
        
        GameManager.Money -= CurrentUpgrade.GetCost();
        currentMonkeyInUpgradesMenu.SetMonkeySellPrice(currentMonkeyInUpgradesMenu.GetMonkeySellPrice() + CurrentUpgrade.GetCost());
        SellPriceText.text = "Sell Price: " + currentMonkeyInUpgradesMenu.GetMonkeySellPrice();
        CurrentUpgrade.UpgradeTower();
        
        //before change
        // if (currentMonkeyInUpgradesMenu.GetUpgradePath2().Count == 0 || !CanBuyUpgradePath2()) 
        //      return;
        // //UpgradePath2Text.text = currentMonkeyInUpgradesMenu.GetUpgradePath2()[1].GetDescription(); //this is obvi an error cuz ur checkin spot [1] which
            //doesnt exist if there is only 1 upgrade left
        // GameManager.Money -= currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].GetCost();
        // currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].UpgradeTower();
    }
    
    public void SetMonkeyPrefab(GameObject newMonkeyPrefab)
    {
        monkeyPrefab = newMonkeyPrefab;
    }
    
    public void StartPlacingMonkey()
    {
        isPlacingMonkey = true;
    }
    
    public bool CanBuyUpgradePath1()
    {
        return GameManager.Money >= currentMonkeyInUpgradesMenu.GetUpgradePath1()[0].GetCost();
    }
    
    public bool CanBuyUpgradePath2()
    {
        return GameManager.Money >= currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].GetCost();
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

    public void SellTower()
    {
        //update gamemanager money
        //destroy tower
        //update tile?
        //close upgrade menu for the monkey
    }
}
