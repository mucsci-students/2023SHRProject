using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonToggleScript : MonoBehaviour
{
    [SerializeField] private Sprite[] buttonSprites;

    [SerializeField] private Image currentButton;

    public void toggleSprite()
    {
        if (currentButton.sprite == buttonSprites[0])
        {
            currentButton.sprite = buttonSprites[1];
            return;
        }

        currentButton.sprite = buttonSprites[0];
    }
}
