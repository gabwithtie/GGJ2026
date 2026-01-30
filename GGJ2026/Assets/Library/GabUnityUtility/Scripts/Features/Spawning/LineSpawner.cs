using System.Collections;
using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    public enum SpawnMode { Random, PingPong, Loop }

    [Header("References")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Settings")]
    [SerializeField] private SpawnMode mode = SpawnMode.Loop;
    [SerializeField] private float interval = 1.0f;
    [SerializeField] private float t_interval = 0.2f;
    [SerializeField] private int countPerInterval = 1;
    [SerializeField] private Vector3 spawn_eulerangles = Vector3.zero;


    private float _pingPongT = 0f;
    private bool _pingPongReverse = false;
    private float _loopT = 0f;

    void Start()
    {
        if (prefab != null && pointA != null && pointB != null)
        {
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogWarning("LineSpawner: Missing references! Please assign points and prefab.");
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            for (int i = 0; i < countPerInterval; i++)
            {
                SpawnObject();
            }
        }
    }

    void SpawnObject()
    {
        float t = GetInterpolationValue();
        Vector3 spawnPosition = Vector3.Lerp(pointA.position, pointB.position, t);
        Instantiate(prefab, spawnPosition, Quaternion.Euler(spawn_eulerangles));
    }

    float GetInterpolationValue()
    {
        switch (mode)
        {
            case SpawnMode.Random:
                return Mathf.Round(Random.value / t_interval) * t_interval;

            case SpawnMode.PingPong:
                // Move back and forth incrementally (simple demonstration)
                if (!_pingPongReverse)
                {
                    _pingPongT += t_interval;
                    if (_pingPongT >= 1f) _pingPongReverse = true;
                }
                else
                {
                    _pingPongT -= t_interval;
                    if (_pingPongT <= 0f) _pingPongReverse = false;
                }
                return Mathf.Clamp01(_pingPongT);

            case SpawnMode.Loop:
                _loopT += t_interval;
                if (_loopT > 1f) _loopT = 0f;
                return _loopT;

            default:
                return 0.5f;
        }
    }

    // --- Gizmos ---
    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        // Draw the path line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);

        // Draw spheres at the ends
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pointA.position, 0.3f);
        Gizmos.DrawWireSphere(pointB.position, 0.3f);

        // Draw a helper label if in Editor
#if UNITY_EDITOR
        UnityEditor.Handles.Label(pointA.position, "Point A");
        UnityEditor.Handles.Label(pointB.position, "Point B");
#endif
    }
}