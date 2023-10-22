using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;
    
    // Start is called before the first frame update
    void Start()
    {
        if(roundText == null)
        {
            Debug.Log("RoundText is null on " + gameObject.name);
            roundText = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        roundText.text = WaveManager.CurrentWaveNumber == 0 
            ? "1" 
            : WaveManager.CurrentWaveNumber.ToString();
    }
}
