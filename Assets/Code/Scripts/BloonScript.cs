using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloonScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rigidbody2D;
    
    /// <summary>
    /// Speed of the bloon. It will be consistent in X/Y direction and always greater than 0, which removes the need for a Vector2 type.
    /// </summary>
    [SerializeField] private uint speed;

    // Start is called before the first frame update
    private void Start()
    {
        if (Rigidbody2D == null)
        {
            Debug.LogWarning("Missing direct link on " + gameObject.name);
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }
        //Rigidbody2D.velocity = new Vector2(speed, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public uint GetSpeed()
    {
        return speed;
    }
}
