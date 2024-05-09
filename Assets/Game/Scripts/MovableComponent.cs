using System.Collections;
using UnityEngine;

public class MovableComponent : MonoBehaviour
{
    private IEnumerator _moveCoroutine;
    
    public void Move(Vector2 position, float time)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }

        _moveCoroutine = MoveCoroutine(position, time);
        StartCoroutine(_moveCoroutine);
    }

    private IEnumerator MoveCoroutine(Vector2 position, float time)
    {
        var startPos = transform.position;

        for (float t = 0; t <= 1 * time; t+= Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPos, position, t / time);
            yield return 0;
        }

        transform.position = position;
    }
}
