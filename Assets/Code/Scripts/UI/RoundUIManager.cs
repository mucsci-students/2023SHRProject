using TMPro;
using UnityEngine;

public class RoundUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private WaveManager waveManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if(roundText == null)
        {
            //Debug.Log("RoundText is null on " + gameObject.name);
            roundText = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        roundText.text = waveManager.CurrentWaveNumber == 0 
            ? "1" 
            : waveManager.CurrentWaveNumber.ToString();
    }
}
