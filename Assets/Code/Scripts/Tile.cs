using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offSetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    public void Init(bool isOffsetColor)
    {
        renderer.color = isOffsetColor ? offSetColor : baseColor;
    }

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }
    
    void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
