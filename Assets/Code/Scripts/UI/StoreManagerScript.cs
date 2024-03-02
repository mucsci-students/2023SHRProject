using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class MonkeySpawner : MonoBehaviour
{
    private GameObject _monkeyPrefab;
    private GameObject _currentMonkey;
    private bool _isPlacingMonkey;
    private MonkeyScript _currentMonkeyInUpgradesMenu;
    private const int SellBackRate = 70;

    [Header("General Object Links")]
    [SerializeField] private GameManager gameManager;
    public Transform projectileContainer;
    
    [FormerlySerializedAs("UpgradeMenuCanvas")]
    [Header("Upgrades Menu Object Links")]
    [SerializeField] private GameObject upgradeMenuCanvas;
    [SerializeField] private Image monkeyImage;
    [SerializeField] private TMP_Dropdown targetingModeDropdown;
    
    [Header("Upgrades Menu Text Links")]
    [SerializeField] private TextMeshProUGUI monkeyNameText;
    [SerializeField] private TextMeshProUGUI sellPriceText;
    
    [Header("Upgrade Path 1 Text Links")]
    [SerializeField] private TextMeshProUGUI upgradePath1DescriptionText;
    [SerializeField] private TextMeshProUGUI upgradePath1CostText;
    
    [Header("Upgrade Path 2 Text Links")]
    [SerializeField] private TextMeshProUGUI upgradePath2DescriptionText;
    [SerializeField] private TextMeshProUGUI upgradePath2CostText;
    
      [Header("Monkey Purchase Buttons")]
      [SerializeField] private List<Button> monkeyButtons = new List<Button>();

     private void Start()
     {
         StartCoroutine(UpdateButtonInteractabilityCoroutine());
         //event for when money is updated
         GameManager.OnMoneyUpdated += UpdateMonkeyButtonsInteractability;
     }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && _currentMonkey != null)
        {
            _isPlacingMonkey = false;
            Destroy(_currentMonkey);
            _currentMonkey = null;
        }
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (_currentMonkey != null)
        {
            _currentMonkey.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Tile tile) && !tile.ContainsTowers())
            {
                _currentMonkey.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0);
            }
        }
        
        if (!Input.GetMouseButtonDown(0))
            return;

        if (hit.collider == null)
            return;

        HandleClick(hit);
        
        UpdateMonkeyButtonsInteractability();
    }
    
    private IEnumerator UpdateButtonInteractabilityCoroutine()
    {
        while (true)
        {
            //will cause an error at star/ or in middle of game if there are no monkeys on the map
            yield return new WaitForSeconds(0.5f);
            UpdateMonkeyButtonsInteractability();
        }
    }
    
    private void UpdateMonkeyButtonsInteractability()
    {
        foreach (Button button in monkeyButtons)
        {
            if (button != null)
            {
                //basically: if you dont have money then turn off the button
                button.interactable =
                    (int.Parse(button.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text.Substring(1)) <=
                     gameManager.money);
            }
            else
            {
                Debug.LogWarning("Button reference is null.");
            }
        }
    }
    
    //prob unnecessary since we are deactivating the button now^^
    private void HandleClick(RaycastHit2D hit)
    {
        if (_isPlacingMonkey)
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
                _isPlacingMonkey = false;
                Destroy(_currentMonkey);
                _currentMonkey = null;
                return;
            }
                    
            PlaceMonkeyOnTile(tile);
        }
    }

    private void HandleMonkeyClick(MonkeyScript monkeyScript)
    {
        // If we already have that monkey open in the upgrade menu and we clicked the monkey again, close it
        if (upgradeMenuCanvas.activeInHierarchy && _currentMonkeyInUpgradesMenu == monkeyScript)
        {
            closeUpgradeMenu();
            _currentMonkeyInUpgradesMenu = null;
        }
        else
        {
            // We clicked a monkey and we are not placing a tower so show upgrade menu
            showUpgradeCanvas(monkeyScript);
            
            if (_currentMonkeyInUpgradesMenu != null)
            {
                _currentMonkeyInUpgradesMenu.SetIsShowingRadius(false);
            }
            
            _currentMonkeyInUpgradesMenu = monkeyScript;
        }
    }
    
    public void showUpgradeCanvas(MonkeyScript currentMonkey)
    {
        upgradeMenuCanvas.SetActive(true);
        
        monkeyNameText.text = currentMonkey.GetMonkeyName();
        monkeyImage.sprite = currentMonkey.GetMonkeyImage();

        var noUpgradesText = "No more upgrades";
        var noUpgradesCostText = "";
        
        upgradePath1DescriptionText.text = currentMonkey.GetUpgradePath1().Count == 0 ? noUpgradesText : currentMonkey.GetUpgradePath1()[0].GetDescription();
        upgradePath1CostText.text = currentMonkey.GetUpgradePath1().Count == 0 ? noUpgradesCostText : "$" + currentMonkey.GetUpgradePath1()[0].GetCost();
        
        upgradePath2DescriptionText.text = currentMonkey.GetUpgradePath2().Count == 0 ? noUpgradesText : currentMonkey.GetUpgradePath2()[0].GetDescription();
        upgradePath2CostText.text = currentMonkey.GetUpgradePath2().Count == 0 ? noUpgradesCostText : "$" + currentMonkey.GetUpgradePath2()[0].GetCost();
        
        sellPriceText.text = "Sell $" + (currentMonkey.GetMonkeySellPrice() * SellBackRate)/100;
        
        targetingModeDropdown.value = (int)currentMonkey.GetTargetingMode();
    }
    
    private string CreateDescriptionText(Upgrade upgrade)
    {
        string description = upgrade.GetDescription();
        description += " Cost: " + upgrade.GetCost();
        return description;
    }
    
    public void OnTargetingModeDropdownValueChanged()
    {
        if (_currentMonkeyInUpgradesMenu != null)
        {
            int selectedValue = targetingModeDropdown.value;
            Enums.TargetingMode newTargetingMode = (Enums.TargetingMode)selectedValue;
            _currentMonkeyInUpgradesMenu.SetTargetingMode(newTargetingMode);
        }
    }

    public void PurchaseUpgrade(List<Upgrade> upgradePath, Func<bool> canBuyUpgrade)
    {
        if (upgradePath.Count == 0 || !canBuyUpgrade())
            return;

        Upgrade currentUpgrade = upgradePath[0];

        gameManager.money -= currentUpgrade.GetCost();
        _currentMonkeyInUpgradesMenu.SetMonkeySellPrice(_currentMonkeyInUpgradesMenu.GetMonkeySellPrice() + currentUpgrade.GetCost());
        currentUpgrade.UpgradeTower();

        showUpgradeCanvas(_currentMonkeyInUpgradesMenu);
        //UpdateMonkeyButtonsInteractability();
    }

    public void purchaseUpgradePath1()
    {
        PurchaseUpgrade(_currentMonkeyInUpgradesMenu.GetUpgradePath1(), CanBuyUpgradePath1);
    }

    public void purchaseUpgradePath2()
    {
        PurchaseUpgrade(_currentMonkeyInUpgradesMenu.GetUpgradePath2(), CanBuyUpgradePath2);
    }
    
    private void closeUpgradeMenu()
    {
        //I removed the button from the menu so this is not needed
        upgradeMenuCanvas.SetActive(false);
    }
    
    public void SetMonkeyPrefab(GameObject newMonkeyPrefab)
    {
        if (_currentMonkey != null)
            Destroy(_currentMonkey);
        
        _monkeyPrefab = newMonkeyPrefab;
        _currentMonkey = Instantiate(newMonkeyPrefab);
        _currentMonkey.GetComponent<BoxCollider2D>().enabled = false;
        _currentMonkey.GetComponent<MonkeyScript>().enabled = false;
    }
    
    public void StartPlacingMonkey()
    {
        _isPlacingMonkey = true;
    }
    
    public bool CanBuyUpgradePath1()
    {
        return gameManager.money >= _currentMonkeyInUpgradesMenu.GetUpgradePath1()[0].GetCost();
    }
    
    public bool CanBuyUpgradePath2()
    {
        return gameManager.money >= _currentMonkeyInUpgradesMenu.GetUpgradePath2()[0].GetCost();
    }

    public bool CanBuyTower()
    {
        return gameManager.money >= _monkeyPrefab.GetComponent<MonkeyScript>().GetMonkeyCost();
    }

    public bool PlaceMonkeyOnTile(Tile tile)
    {
        if (tile.ContainsTowers())
            return false;
        
        Vector3 tilePosition = tile.transform.position;
        tilePosition.z = 0;
                    
        //GameObject newMonkey = Instantiate(monkeyPrefab, tilePosition, Quaternion.identity);
        _currentMonkey.transform.SetPositionAndRotation(tilePosition, Quaternion.identity);
        _isPlacingMonkey = false; //no more monkey for you!
                    
        tile.SetContainsTower(true);
        _currentMonkey.GetComponent<MonkeyScript>().SetTile(tile);
        gameManager.money -= _monkeyPrefab.GetComponent<MonkeyScript>().GetMonkeyCost();

        _currentMonkey.GetComponent<BoxCollider2D>().enabled = true;
        _currentMonkey.GetComponent<MonkeyScript>().enabled = true;
        _currentMonkey.GetComponent<MonkeyScript>().SetProjectileContainer(projectileContainer);
        _currentMonkey.transform.parent = gameObject.transform;
        _currentMonkey = null;
        return true;
    }

    public void DestroyAllMonkeys()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    
    public void DestroyAllProjectiles()
    {
        foreach (Transform child in projectileContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void SellTower()
    {
        gameManager.money += (_currentMonkeyInUpgradesMenu.GetMonkeySellPrice()*SellBackRate)/100;
        _currentMonkeyInUpgradesMenu.GetTile().SetContainsTower(false);
        Destroy(_currentMonkeyInUpgradesMenu.gameObject);
        upgradeMenuCanvas.SetActive(false);
        //UpdateMonkeyButtonsInteractability();
    }
}
