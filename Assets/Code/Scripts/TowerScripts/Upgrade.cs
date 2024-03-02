using System;
using UnityEngine;

[Serializable]
public class Upgrade
{
    // Create setter and getters for these
    
    [SerializeField] private int cost;
    [SerializeField] private int tier;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    private Action _upgrade;

    public void UpgradeTower()
    {
        _upgrade();
    }
    
    public int GetCost()
    {
        return cost;
    }
    
    public int GetTier()
    {
        return tier;
    }
    
    public string GetDescription()
    {
        return description;
    }
    
    public Sprite GetSprite()
    {
        return sprite;
    }
    
    public void SetUpgrade(Action newUpgrade)
    {
        _upgrade = newUpgrade;
    }
}
