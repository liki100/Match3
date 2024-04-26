using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GamePiece), typeof(Animator))]
public class ClearablePiece : MonoBehaviour
{
    [SerializeField] private AnimationClip _clearAnimation;

    private bool _isBeingCleaned;

    public bool IsBeingCleaned => _isBeingCleaned;

    protected GamePiece _piece;
    private Animator _animator;

    private void Awake()
    {
        _piece = GetComponent<GamePiece>();
        _animator = GetComponent<Animator>();
    }

    public virtual void Clear()
    {
        _isBeingCleaned = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        _animator.Play(_clearAnimation.name);
        yield return new WaitForSeconds(_clearAnimation.length);
        
        Destroy(_piece.gameObject);
    }
}
