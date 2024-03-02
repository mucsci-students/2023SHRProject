using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonkeyScript : MonoBehaviour
{
    #region Fields

    private readonly List<GameObject> _enemiesInRange = new();

    [Header("Tower Settings")] 
    
    [SerializeField]
    private string monkeyName;

    [SerializeField] private int monkeyCost;

    private int _sellPrice;
    
    [SerializeField] 
    [Tooltip("Measured as seconds between firing.")]
    [Min(0.01f)]
    protected float firingRate = 1f;

    [SerializeField] 
    protected Enums.TargetingMode targetingMode = Enums.TargetingMode.First; 
    
    [SerializeField]
    [Range(0, 720)]
    [Tooltip("Rotation speed in angular degrees per second")]
    private float rotateSpeed = 270f;
    
    [SerializeField]
    [Tooltip("Adds a small variance to firing rate to make towers shoot at slightly different times.")]
    protected float shootingVariance = 0.05f;

    protected float actualVariance;

    [SerializeField]
    protected List<Upgrade> upgradePath1 = new(2);
    
    [SerializeField]
    protected List<Upgrade> upgradePath2 = new(2);

    [Header("Projectile Settings")]
    
    [SerializeField]
    protected float projectileSpeed = 1f;
    [SerializeField]
    protected float maxProjectileDistance = 10f;
    [SerializeField]
    [Min(0)]
    protected uint layersPoppedPerHit = 1;
    [SerializeField]
    protected uint pierceAmount = 1;

    [Header("Projectile")] 
    
    [SerializeField]
    protected GameObject projectilePrefab;
    
    [Header("Stats")] 
    
    [SerializeField]
    private int totalLayersPopped = 0;

    [Header("Object Links")] 
    
    [SerializeField]
    protected SpriteRenderer radiusSpriteRenderer;

    [SerializeField]
    protected CircleCollider2D radiusCollider;

    [SerializeField] protected Transform projectileContainer;
         
    private float _timer = 0f;
    
    private Sprite MonkeyImage;

    private Tile currentTile;
    
    #endregion
    
    #region Unity Functions

    protected virtual void Start()
    {
        MonkeyImage = GetComponent<SpriteRenderer>().sprite;
        SetIsShowingRadius(false);
        PopulateUpgrades();
        _sellPrice = monkeyCost;
        actualVariance = UnityEngine.Random.Range(0, shootingVariance);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_enemiesInRange.Count > 0)
        {
            var target = GetTarget(targetingMode);
            if (target == null || _enemiesInRange.Count == 0)
            {
                return;
            }
            
            LookAt(target.transform.position);
            if (_timer >= firingRate - actualVariance)
            {
                Fire(target);
                _timer = 0;
            }
        }
    }
    
    private void OnMouseDown()
    {
        ToggleIsShowingRadius();
    }

    #endregion

    #region Targeting Modes/Firing and Logic
    
    protected virtual void Fire(GameObject target)
    { 
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.transform.parent = projectileContainer;
        var projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.SetAllAttributes(projectileSpeed, maxProjectileDistance, layersPoppedPerHit, pierceAmount, target, this);
    }
    
    protected virtual void LookAt(Vector3 targetPosition)
    {
        Vector3 myLocation = transform.position;
        targetPosition.z = myLocation.z; // ensure there is no 3D rotation by aligning Z position
 
        Vector3 vectorToTarget = targetPosition - myLocation;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 0) * vectorToTarget;
 
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);

        // Angular speed in degrees per sec.
        var maxRotateSpeed = rotateSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotateSpeed);
    }

    private GameObject GetTarget(Enums.TargetingMode currentTargetingMode)
    {
        return currentTargetingMode switch
        {
            Enums.TargetingMode.First => GetFirstTarget(),
            Enums.TargetingMode.Last => GetLastTarget(),
            Enums.TargetingMode.Strongest => GetStrongestTarget(),
            Enums.TargetingMode.None => GetFirstTarget(),
            _ => GetFirstTarget()
        };
    }

    private GameObject GetFirstTarget()
    {
        GameObject target = null;
        var maxDistanceTravelled = Mathf.NegativeInfinity;

        for (int i = 0; i < _enemiesInRange.Count; ++i)
        {
            GameObject enemy = _enemiesInRange[i];            
            
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                --i;
                continue;
            }
            
            var distanceTravelled = enemy.GetComponent<PathFollowingScript>().GetDistanceTravelled();
            if (distanceTravelled > maxDistanceTravelled)
            {
                maxDistanceTravelled = distanceTravelled;
                target = enemy;
            }
        }
        
        return target;
    }

    private GameObject GetLastTarget()
    {
        GameObject target = null;
        var leastDistanceTravelled = Mathf.Infinity;

        for (int i = 0; i < _enemiesInRange.Count; ++i)
        {
            GameObject enemy = _enemiesInRange[i];
            
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                --i;
                continue;
            }
            
            var distanceTravelled = enemy.GetComponent<PathFollowingScript>().GetDistanceTravelled();
            if (distanceTravelled < leastDistanceTravelled)
            {
                leastDistanceTravelled = distanceTravelled;
                target = enemy;
            }
        }
        
        return target;
    }

    private GameObject GetStrongestTarget()
    {
        GameObject target = null;
        var strongestHealth = Mathf.NegativeInfinity;
        var maxDistanceTravelled = Mathf.NegativeInfinity;
        const float tolerance = 0.1f;

        for (int i = 0; i < _enemiesInRange.Count; ++i)
        {
            GameObject enemy = _enemiesInRange[i];
            
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                --i;
                continue;
            }
            
            var distanceTravelled = enemy.GetComponent<PathFollowingScript>().GetDistanceTravelled();
            var enemyHealth = enemy.GetComponent<BloonScript>().GetHealth();
            if (enemyHealth > strongestHealth)
            {
                strongestHealth = enemyHealth;
                maxDistanceTravelled = distanceTravelled;
                target = enemy;
            } else if (Math.Abs(strongestHealth - enemyHealth) < tolerance && distanceTravelled > maxDistanceTravelled)
            {
                strongestHealth = enemyHealth;
                maxDistanceTravelled = distanceTravelled;
                target = enemy;
            }
        }
        
        return target;
    }
    
    #endregion

    #region Getters and Setters
    
    public void SetProjectileContainer(Transform newTransform)
    {
        projectileContainer = newTransform;
    }

    public string GetMonkeyName()
    {
        return monkeyName;
    }

    public Sprite GetMonkeyImage()
    {
        return MonkeyImage;
    }
    
    public Enums.TargetingMode GetTargetingMode()
    {
        return targetingMode;
    }
    
    public void SetTargetingMode(Enums.TargetingMode newTargetingMode)
    {
        targetingMode = newTargetingMode;
    }
    
    public void IncrementLayersPopped(int layersPopped)
    {
        totalLayersPopped += layersPopped;
    }
    
    public void AddBloonToRange(GameObject bloon)
    {
        _enemiesInRange.Add(bloon);
    }
    
    public void TryAddBloonToRange(GameObject bloon)
    {
        if (!_enemiesInRange.Contains(bloon))
            _enemiesInRange.Add(bloon);
    }
    
    public void RemoveBloonFromRange(GameObject bloon)
    {
        _enemiesInRange.Remove(bloon);
    }

    public void ToggleIsShowingRadius()
    {
        SetIsShowingRadius(!radiusSpriteRenderer.enabled);
    }

    public void SetIsShowingRadius(bool state)
    {
        radiusSpriteRenderer.enabled = state;
    }

    public int GetMonkeyCost()
    {
        return monkeyCost;
    }

    public int GetMonkeySellPrice()
    {
        return _sellPrice;
    }
    
    public void SetMonkeySellPrice(int newSellPrice)
    {
        _sellPrice = newSellPrice;
    }
    
    public List<Upgrade> GetUpgradePath1()
    {
        return upgradePath1;
    }
    
    public List<Upgrade> GetUpgradePath2()
    {
        return upgradePath2;
    }

    public Tile GetTile()
    {
        return currentTile;
    }
    
    public void SetTile(Tile tile)
    {
        currentTile = tile;
    }
    #endregion

    #region Upgrades and Logic

    private void PopulateUpgrades()
    {
        upgradePath1[0].SetUpgrade(Upgrade1_1Helper);
        upgradePath1[1].SetUpgrade(Upgrade1_2Helper);
        upgradePath2[0].SetUpgrade(Upgrade2_1Helper);
        upgradePath2[1].SetUpgrade(Upgrade2_2Helper);
    }
    
    private void Upgrade1_1Helper()
    {
        if (upgradePath1.Count == 0)
            return;
        
        Upgrade1_1();
        
        GetComponent<SpriteRenderer>().sprite = upgradePath1[0].GetSprite();
        upgradePath1.RemoveAt(0);
    }
    
    private void Upgrade1_2Helper()
    {
        if (upgradePath1.Count == 0)
            return;
        
        Upgrade1_2();
        
        GetComponent<SpriteRenderer>().sprite = upgradePath1[0].GetSprite();
        upgradePath1.RemoveAt(0);
    }
    
    private void Upgrade2_1Helper()
    {
        if (upgradePath2.Count == 0)
            return;
        
        Upgrade2_1();
        
        GetComponent<SpriteRenderer>().sprite = upgradePath2[0].GetSprite();
        upgradePath2.RemoveAt(0);
    }
    
    private void Upgrade2_2Helper()
    {
        if (upgradePath2.Count == 0)
            return;
        
        Upgrade2_2();
        
        GetComponent<SpriteRenderer>().sprite = upgradePath2[0].GetSprite();
        upgradePath2.RemoveAt(0);
    }

    protected abstract void Upgrade1_1();

    protected abstract void Upgrade1_2();

    protected abstract void Upgrade2_1();

    protected abstract void Upgrade2_2();
    
    // Testing
    
    [ContextMenu("Upgrade 1")]
    public void Upgrade1Test()
    {
        upgradePath1[0].UpgradeTower();
    }
    
    [ContextMenu("Upgrade 2")]
    public void Upgrade2Test()
    {
        upgradePath2[0].UpgradeTower();
    }

    #endregion
}
