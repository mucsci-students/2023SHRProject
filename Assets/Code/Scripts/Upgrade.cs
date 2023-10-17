using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Upgrade
{
    // Create setter and getters for these
    
    [SerializeField] private int cost;
    [SerializeField] private int tier;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    private Action upgrade;

    public void UpgradeTower()
    {
        upgrade();
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
    
    public void SetUpgrade(Action upgrade)
    {
        this.upgrade = upgrade;
    }
}
