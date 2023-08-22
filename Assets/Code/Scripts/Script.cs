using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Vector2 Speed;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 2, 0);
        _rigidbody2D.velocity = Speed;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
