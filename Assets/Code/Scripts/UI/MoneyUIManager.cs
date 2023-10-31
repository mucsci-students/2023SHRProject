using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary> Updates the lives text on the UI </summary>
public class MoneyUIManager : MonoBehaviour
{
    
    /// <summary> A reference to the lives text object </summary>
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private GameManager gameManager;
    
    /// <summary> Unity event function called when the script is loaded </summary>
    private void Start()
    {
        if (money == null)
        {
            Debug.Log("LivesText is null on " + gameObject.name);
            money = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary> Unity event function, called once per frame </summary>
    private void Update()
    {
        money.text = gameManager.Money.ToString();
    }
}