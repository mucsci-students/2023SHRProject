using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    public GameObject otherCube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2 (0f, 1f));
        otherCube.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 1f));

        if (transform.position.y > 20f)
        {
            Destroy(gameObject);
        }
    }
}
