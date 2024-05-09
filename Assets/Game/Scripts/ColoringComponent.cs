using System.Collections.Generic;
using UnityEngine;

public class ColoringComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ItemColor[] _colors;
    
    private ColorType _type;

    public ColorType Type => _type;
    public int Count => _colorDictionary.Count;

    private readonly Dictionary<ColorType, Sprite> _colorDictionary = new();

    private void Awake()
    {
        foreach (var color in _colors)
        {
            _colorDictionary.TryAdd(color.Type, color.Sprite);
        }
    }
    
    public void Coloring(ColorType type)
    {
        _type = type;
        _spriteRenderer.sprite = _colorDictionary[type];
    }
}

[System.Serializable]
public class ItemColor
{
    [SerializeField] private ColorType _type;
    [SerializeField] private Sprite _sprite;

    public ColorType Type => _type;
    public Sprite Sprite => _sprite;
}
