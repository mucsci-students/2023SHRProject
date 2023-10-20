using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite baseSprite, offSetSprite;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    
    private bool containsTower = false;

    public void Init(bool isOffsetColor)
    {
        renderer.sprite = isOffsetColor ? offSetSprite : baseSprite;
    }

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }
    
    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    public bool ContainsTowers()
    {
        return containsTower;
    }
    
    public void SetContainsTower(bool state)
    {
        containsTower = state;
    }
    
}
