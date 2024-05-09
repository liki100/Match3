using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private AnimationClip _clearAnimation;

    protected bool _isBeingCleaned;
    
    [SerializeField] private ItemType _type;
    public ItemType Type => _type;

    private MovableComponent _movableComponent;
    public MovableComponent MovableComponent => _movableComponent;
    
    private ColoringComponent _coloringComponent;
    public ColoringComponent ColoringComponent => _coloringComponent;
    
    private void Awake()
    {
        _movableComponent = GetComponent<MovableComponent>();
        _coloringComponent = GetComponent<ColoringComponent>();
    }

    public bool IsMovable()
    {
        return _movableComponent != null;
    }

    public bool IsColored()
    {
        return _coloringComponent != null;
    }

    public virtual void Clear(GridNew grid, int x, int y)
    {
        if (_isBeingCleaned)
            return;

        _isBeingCleaned = true;
        StartCoroutine(ClearCoroutine());
    }
    
    private IEnumerator ClearCoroutine()
    {
        //_animator.Play(_clearAnimation.name);
        //yield return new WaitForSeconds(_clearAnimation.length);
        yield return new WaitForSeconds(.15f);
        Destroy(gameObject);
    }
}
