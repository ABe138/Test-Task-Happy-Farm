using System.Collections;
using UnityEngine;

public class CollectableItem : MonoBehaviour, ICollectable
{
    [SerializeField] private float _timeToLive = 5f;

    [SerializeField] private Collider _collider;

    private float _ttlCurrent;

    private Coroutine _moveCoroutine;

    private string _collectableItemId;

    public string Id => _collectableItemId;

    private void Update()
    {
        _ttlCurrent += Time.deltaTime;
        if (_ttlCurrent > _timeToLive) PoolingManager.Instance.Release(this);
    }

    private void OnDisable()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
    }

    public void Initialize(Vector3 targetPosition, string collectableItemId)
    {
        _collectableItemId = collectableItemId;
        _collider.enabled = true;
        transform.position = targetPosition;
        _ttlCurrent = 0;
    }

    public void Collect(ICollector collector)
    {
        if (collector.Consume(this)) 
        {
            _collider.enabled = false;
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(MoveToCollector(collector));
        }
    }

    private IEnumerator MoveToCollector(ICollector collector)
    {
        var currentTime = 0f;
        var moveDuration = 0.5f;
        var initialPosition = transform.position;
        while (currentTime < moveDuration)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, collector.CollectorTransform.position + Vector3.up, currentTime / moveDuration);
            yield return null;
        }
        transform.position = collector.CollectorTransform.position + Vector3.up;
        collector.Consume(this);
        PoolingManager.Instance.Release(this);
    }

    public Vector3 GetPosition() => transform.position;
}
