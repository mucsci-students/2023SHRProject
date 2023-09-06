using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManagerScript : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                Mathf.Infinity);
 
            if(hit.collider != null)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (gameObject.TryGetComponent<MonkeyScript>(out var monkeyScript))
                {
                    monkeyScript.ToggleIsShowingRadius();
                }
            }
        }
    }
}
