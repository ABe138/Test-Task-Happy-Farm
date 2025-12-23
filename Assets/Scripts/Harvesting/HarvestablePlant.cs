using System.Collections;
using System.Linq;
using UnityEngine;

public class HarvestablePlant : MonoBehaviour, IHarvestable
{
    [SerializeField] private HarvestableConfig _config;
    
    [SerializeField] private Collider _hitCollider;
    [SerializeField] private Transform _scaleAnchor;
    [SerializeField] private Transform _tiltAnchor;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _rebirthEffect;

    private int _durabilityCurrent;

    private void Awake()
    {
        _durabilityCurrent = _config.Durability;
    }

    public bool Harvest(Vector3 hitFrom)
    {
        if (_durabilityCurrent <= 0) return false;
        _durabilityCurrent--;
        OnHarvested();
        PlayHitTilt(hitFrom);
        if (_durabilityCurrent <= 0) OnDepleted();
        return true;
    }

    protected virtual void OnHarvested()
    {
        _hitEffect.SetActive(false);
        _hitEffect.SetActive(true);

        var item = PoolingManager.Instance.Pool(_config.Produce.CollectableItemPrefab, null, Vector3.zero, Quaternion.identity);
        var randomRange = Random.insideUnitCircle;
        item.Initialize(transform.position + new Vector3(randomRange.x, 0, randomRange.y) * _config.ProduceSpread, _config.Produce.Id);
    }

    protected virtual void OnDepleted()
    {
        _hitCollider.enabled = false;
        _scaleAnchor.localScale = Vector3.one * 0.01f;
        StartCoroutine(Regrow());
    }

    public void PlayHitTilt(Vector3 hitWorldPosition)
    {
        StopCoroutine("TiltCoroutine");
        StartCoroutine(TiltCoroutine(hitWorldPosition));
    }

    private IEnumerator TiltCoroutine(Vector3 hitWorldPosition)
    {
        var maxTiltAngle = 15f;
        var tiltInDuration = 0.06f;   // time to tilt toward hit
        var tiltOutDuration = 0.15f;  // time to return to identity

        var localHitDir = _tiltAnchor.InverseTransformDirection((hitWorldPosition - _tiltAnchor.position).normalized);

        var tiltX = Mathf.Clamp(-localHitDir.z, -1f, 1f); // forward/back affects X rotation (negated so forward hit tilts nose down)
        var tiltZ = Mathf.Clamp(localHitDir.x, -1f, 1f);  // right/left affects Z rotation

        var startRot = _tiltAnchor.localRotation;
        var targetRot = Quaternion.Euler(tiltX * maxTiltAngle, 0f, tiltZ * maxTiltAngle) * startRot;

        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(tiltInDuration, 0.0001f);
            _tiltAnchor.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        t = 0f;
        var returnStart = _tiltAnchor.localRotation;
        var returnTarget = Quaternion.identity;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(tiltOutDuration, 0.0001f);
            _tiltAnchor.localRotation = Quaternion.Slerp(returnStart, returnTarget, t);
            yield return null;
        }

        _tiltAnchor.localRotation = returnTarget;
    }

    private IEnumerator Regrow()
    {
        var currentTime = 0f;
        while (currentTime < _config.RegrowDuration)
        {
            currentTime += Time.deltaTime;
            _scaleAnchor.localScale = Vector3.one * Mathf.Lerp(0.01f, 1f, currentTime / _config.RegrowDuration);
            yield return null;
        }
        _scaleAnchor.localScale = Vector3.one;
        _hitCollider.enabled = true;
        _durabilityCurrent = _config.Durability;

        _rebirthEffect.SetActive(false);
        _rebirthEffect.SetActive(true);
    }

    public Vector3 GetPosition() => transform.position;
}
