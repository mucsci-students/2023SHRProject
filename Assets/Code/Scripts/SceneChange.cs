using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
