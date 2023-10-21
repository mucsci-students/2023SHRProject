using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonkeySpawner : MonoBehaviour
{
    private GameObject monkeyPrefab;
    private GameObject currentMonkey;
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
        if (Input.GetMouseButtonDown(1) && currentMonkey != null)
        {
            isPlacingMonkey = false;
            Destroy(currentMonkey);
            currentMonkey = null;
        }
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (currentMonkey != null)
        {
            currentMonkey.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Tile tile) && !tile.ContainsTowers())
            {
                currentMonkey.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0);
            }
        }
        
        if (!Input.GetMouseButtonDown(0))
            return;

        if (hit.collider == null)
            return;

        HandleClick(hit);
    }

    private void HandleClick(RaycastHit2D hit)
    {
        if (isPlacingMonkey)
        {
            TryPlaceMonkey(hit);
        } 
        else if (hit.collider.gameObject.TryGetComponent(out MonkeyScript monkeyScript))
        {
            HandleMonkeyClick(monkeyScript);
        }
    }

    private void TryPlaceMonkey(RaycastHit2D hit)
    {
        if (hit.collider.gameObject.TryGetComponent(out Tile tile))
        {
            if (!CanBuyTower())
            {
                Debug.Log("no monkey for you");
                isPlacingMonkey = false;
                Destroy(currentMonkey);
                currentMonkey = null;
                return;
            }
                    
            PlaceMonkeyOnTile(tile);
        }
    }

    private void HandleMonkeyClick(MonkeyScript monkeyScript)
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
    
    public void showUpgradeCanvas(MonkeyScript currentMonkey)
    {
        UpgradeMenuCanvas.SetActive(true);
        
        monkeyNameText.text = currentMonkey.GetMonkeyName();
        MonkeyImage.sprite = currentMonkey.GetMonkeyImage();

        var noUpgradesText = "No more upgrades";
        
        UpgradePath1Text.text = currentMonkey.GetUpgradePath1().Count == 0 ? noUpgradesText : CreateDescriptionText(currentMonkey.GetUpgradePath1()[0]);
        UpgradePath2Text.text = currentMonkey.GetUpgradePath2().Count == 0 ? noUpgradesText : CreateDescriptionText(currentMonkey.GetUpgradePath2()[0]);
        
        SellPriceText.text = "Sell Price: " + currentMonkey.GetMonkeySellPrice();
        //check if this runs 2x later

        //add targeting mode thing
        //also update current money if selling
        //also add how many bloons it popped, its popping power, other helpful info for player
    }
    
    private string CreateDescriptionText(Upgrade upgrade)
    {
        string description = upgrade.GetDescription();
        description += " Cost: " + upgrade.GetCost();
        return description;
    }

    
    public void PurchaseUpgrade(List<Upgrade> upgradePath, Func<bool> canBuyUpgrade)
    {
        if (upgradePath.Count == 0 || !canBuyUpgrade())
            return;

        Upgrade currentUpgrade = upgradePath[0];

        GameManager.Money -= currentUpgrade.GetCost();
        currentMonkeyInUpgradesMenu.SetMonkeySellPrice(currentMonkeyInUpgradesMenu.GetMonkeySellPrice() + currentUpgrade.GetCost());
        currentUpgrade.UpgradeTower();

        showUpgradeCanvas(currentMonkeyInUpgradesMenu);
    }

    public void purchaseUpgradePath1()
    {
        PurchaseUpgrade(currentMonkeyInUpgradesMenu.GetUpgradePath1(), CanBuyUpgradePath1);
    }

    public void purchaseUpgradePath2()
    {
        PurchaseUpgrade(currentMonkeyInUpgradesMenu.GetUpgradePath2(), CanBuyUpgradePath2);
    }
    
    private void closeUpgradeMenu()
    {
        //I removed the button from the menu so this is not needed
        UpgradeMenuCanvas.SetActive(false);
    }
    
    public void SetMonkeyPrefab(GameObject newMonkeyPrefab)
    {
        if (currentMonkey != null)
            Destroy(currentMonkey);
        
        monkeyPrefab = newMonkeyPrefab;
        currentMonkey = Instantiate(newMonkeyPrefab);
        currentMonkey.GetComponent<BoxCollider2D>().enabled = false;
        currentMonkey.GetComponent<MonkeyScript>().enabled = false;
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
                    
        //GameObject newMonkey = Instantiate(monkeyPrefab, tilePosition, Quaternion.identity);
        currentMonkey.transform.SetPositionAndRotation(tilePosition, Quaternion.identity);
        isPlacingMonkey = false; //no more monkey for you!
                    
        tile.SetContainsTower(true);
        currentMonkey.GetComponent<MonkeyScript>().SetTile(tile);
        GameManager.Money -= monkeyPrefab.GetComponent<MonkeyScript>().GetMonkeyCost();

        currentMonkey.GetComponent<BoxCollider2D>().enabled = true;
        currentMonkey.GetComponent<MonkeyScript>().enabled = true;
        currentMonkey = null;
        return true;
    }

    public void SellTower()
    {
        GameManager.Money += currentMonkeyInUpgradesMenu.GetMonkeySellPrice();
        currentMonkeyInUpgradesMenu.GetTile().SetContainsTower(false);
        Destroy(currentMonkeyInUpgradesMenu.gameObject);
        UpgradeMenuCanvas.SetActive(false);
    }
}
