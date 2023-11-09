using UnityEngine;
using TMPro;

public class sliderControllerScript : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI sliderText;
  [SerializeField] private float maxSliderAmount = 100.0f;

  public void sliderChange(float value)
  {
    float localValue = value * maxSliderAmount;
    sliderText.text = localValue.ToString("0") + "%"; //updates slider text box
  }
}
