using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageObliteratorBurst : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;

    [Tooltip("Max world-space radius at peak expansion. " +
             "= (StartSize * peak Size curve value * object scale) / 2")]
    [SerializeField] private float maxRadius = 5f;

    [Tooltip("Match this to particle Start Lifetime (0.8)")]
    [SerializeField] private float expandTime = 0.8f;

    [SerializeField] private LayerMask planetLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmo = true;
    private float _currentRadius = 0f;

    // Track already-hit mirages so we don't fade them multiple times
    private HashSet<Objects> _alreadyHit = new HashSet<Objects>();

    private void Start()
    {
        StartCoroutine(ExpandAndDetect());
    }

    private IEnumerator ExpandAndDetect()
    {
        float elapsed = 0f;

        while (elapsed < expandTime)
        {
            elapsed += Time.deltaTime;
            _currentRadius = Mathf.Lerp(0f, maxRadius, elapsed / expandTime);

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, _currentRadius, planetLayer);

            foreach (Collider2D hit in hits)
            {
                Objects obj = hit.GetComponent<Objects>();
                if (obj != null && obj.Mirage && !_alreadyHit.Contains(obj))
                {
                    _alreadyHit.Add(obj);
                    Debug.Log($"[Burst] Obliterated Mirage: {hit.name}");
                    obj.DestroyWithFade(fadeDuration);
                }
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmo) return;

        // Expanding radius — magenta fill
        Gizmos.color = new Color(1f, 0f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, _currentRadius);

        // Expanding radius — magenta outline
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _currentRadius);

        // Max radius preview — yellow outline
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}