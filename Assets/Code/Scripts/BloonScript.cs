using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class BloonScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rigidbody2D;
    [SerializeField] private Vector2 Speed;

    // Start is called before the first frame update
    private void Start()
    {
        if (Rigidbody2D == null)
        {
            Debug.LogWarning("Missing direct link on " + gameObject.name);
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        Rigidbody2D.velocity = new Vector2(Speed.x, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
