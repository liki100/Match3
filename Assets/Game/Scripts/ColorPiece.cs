using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    [SerializeField] private ColorSprite[] _colorSprites;

    private Dictionary<ColorType, Sprite> _colorSpriteDict;
    private SpriteRenderer _spriteRenderer;
    private ColorType _color;

    public int NumColors => _colorSpriteDict.Count;
    public ColorType Color => _color;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (var i = 0; i < _colorSprites.Length; i++)
        {
            _colorSpriteDict.TryAdd(_colorSprites[i].Color, _colorSprites[i].Sprite);
        }
    }

    public void SetColor(ColorType newColor)
    {
        if (!_colorSpriteDict.ContainsKey(newColor)) 
            return;
        
        _color = newColor;
        _spriteRenderer.sprite = _colorSpriteDict[newColor];
    }
}
