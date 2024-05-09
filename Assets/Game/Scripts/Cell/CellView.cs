using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CellView : MonoBehaviour
{
    [SerializeField] private PortalSprite[] _portalSprites;

    private readonly Dictionary<PortalType, Sprite> _portalSpriteDictionary = new();

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        foreach (var portalSprite in _portalSprites)
        {
            _portalSpriteDictionary.TryAdd(portalSprite.Type, portalSprite.Sprite);
        }
    }

    public void SetActive(bool active)
    {
        _spriteRenderer.enabled = active;
    }

    public void SetSprite(PortalType type)
    {
        _spriteRenderer.sprite = _portalSpriteDictionary[type];
    }
}

[System.Serializable]
public class PortalSprite
{
    [SerializeField] private PortalType _type;
    [SerializeField] private Sprite _sprite;

    public PortalType Type => _type;
    public Sprite Sprite => _sprite;
}
