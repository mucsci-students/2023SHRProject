using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite baseSprite, offSetSprite;
    
    private SpriteRenderer _renderer;
    private GameObject _highlight;
    private bool _isOffsetColor;
    private bool _containsTower;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = _isOffsetColor ? offSetSprite : baseSprite;
        _highlight = transform.GetChild(0).gameObject;
    }

    public void SetColor(bool isOffsetColor)
    {
        _isOffsetColor = isOffsetColor;
    }

    private void OnMouseEnter()
    {
        if(_highlight != null) _highlight.SetActive(true);
    }
    
    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    public bool ContainsTowers()
    {
        return _containsTower;
    }
    
    public void SetContainsTower(bool state)
    {
        _containsTower = state;
    }
    
}
