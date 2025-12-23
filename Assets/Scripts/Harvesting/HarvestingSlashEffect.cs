using System.Collections;
using UnityEngine;

public class HarvestingSlashEffect : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private AnimationCurve _arcScaleCurve;

    private MaterialPropertyBlock _materialPropertyBlock;

    private void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void Animate(float arcSweep, float reach, float time) 
    {
        var startAngle = (180f - arcSweep) / 2f;

        _materialPropertyBlock.SetFloat("_FillOrigin", startAngle);
        _materialPropertyBlock.SetFloat("_FillAmount", arcSweep);

        _renderer.SetPropertyBlock(_materialPropertyBlock);

        StartCoroutine(PlayAnimation(reach, time));
    }

    private IEnumerator PlayAnimation(float reach, float time)
    {
        var currentTime = 0f;
        while (currentTime < time) 
        {
            var t = currentTime / time;
            transform.localScale = Vector3.one * Mathf.Lerp(reach / 3f, reach, _arcScaleCurve.Evaluate(t));
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * reach;
        PoolingManager.Instance.Release(this);
    }
}
