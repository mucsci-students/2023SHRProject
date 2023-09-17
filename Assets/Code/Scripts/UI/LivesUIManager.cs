using TMPro;
using UnityEngine;

/// <summary> Updates the lives text on the UI </summary>
public class LivesUIManager : MonoBehaviour
{
    
    /// <summary> A reference to the lives text object </summary>
    [SerializeField]
    private TextMeshProUGUI livesText;
    
    /// <summary> Unity event function called when the script is loaded </summary>
    private void Start()
    {
        if (livesText == null)
        {
            Debug.Log("LivesText is null on " + gameObject.name);
            livesText = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary> Unity event function, called once per frame </summary>
    private void Update()
    {
        livesText.text = GameManager.Lives.ToString();
    }
}
