using UnityEngine;
using UnityEngine.UI;

public class MonkeySpawner : MonoBehaviour
{
    public GameObject monkeyPrefab;
    private bool isPlacingMonkey = false;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(StartPlacingMonkey);
    }

    private void Update()
    {
        if (isPlacingMonkey)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f; //mouse down/click
                Instantiate(monkeyPrefab, mousePosition, Quaternion.identity);
                isPlacingMonkey = false; //no more monkey for you!
            }
        }
    }
    
    private void StartPlacingMonkey() //you can start placin some monkey -- replace with money mechanic!!
    {
        isPlacingMonkey = true;
    }
}



//grid - [10]
//monkey - [0]
//monkey radius - [1]
//monkey projectiles/objects?
//bloons [2 - 3]
    //blue bloon - [2.85]
    //red bloon - [2.9]
//road & path - [3]

//UI - drawn over
