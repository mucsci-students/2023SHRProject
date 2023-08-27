using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Speed of the projectile")]
    private float Speed = 1f;
    
    [SerializeField] 
    [Tooltip("Max distance that the projectile can move before despawning")]
    private float MaxDistance = 10f;
    
    [SerializeField] 
    [Tooltip("Amount of layers popped per hit. a.k.a damage but must be whole number")]
    private uint LayersPerHit = 1;
    
    [SerializeField]
    [Tooltip("Number of bloons that this projectile can hit.")]
    private uint PierceAmount = 1;

    private Vector2 _direction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
