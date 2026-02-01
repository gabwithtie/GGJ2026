using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GabUnity
{
    [System.Serializable]
    public struct ObstacleSettings
    {
        public string Name;
        public GameObject Prefab;

        [Range(1, 100)]
        [Tooltip("Higher priority makes this object more likely to be chosen.")]
        public int Priority;

        [Header("Spacing Rules")]
        [Tooltip("Min distance since THIS specific prefab last spawned.")]
        public float MinimumSelfGap;

        [Tooltip("Min distance since ANY prefab last spawned.")]
        public float MinimumGap;

        [Tooltip("Forced empty space AFTER this specific object spawns.")]
        public float NextObstaclePadding;

        [HideInInspector] public float lastSpawnGlobalDist; // Internal tracker
    }

    public class InfiniteObstacleSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private ObstacleSettings[] obstacles;
        [SerializeField] private float density = 5.0f; // Distance between spawn ticks
        [Range(0f, 1f)][SerializeField] private float globalProbability = 0.5f;

        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private Vector3 startPosition;

        [Header("Movement Settings")]
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private bool use_global = true;
        private float speed => use_global ? InfiniteScrollerManager.ScrollSpeed : _speed;
        [SerializeField] private Vector3 direction = Vector3.back;

        [Header("Cleanup Settings")]
        [SerializeField] private float deleteAtZ = -20.0f;

        private float distanceTravelled;      // Ticks toward the next 'density' check
        private float globalDistanceCounter; // Total absolute distance world has moved
        private float lastSpawnGlobalDist;   // Global distance at which the last spawn occurred
        private float currentRequiredPadding; // Variable padding set by the previous obstacle

        private Queue<Rigidbody> activeObstacles = new Queue<Rigidbody>();

        private void Start()
        {
            direction = direction.normalized;

            // Initialize trackers so objects are eligible immediately
            for (int i = 0; i < obstacles.Length; i++)
            {
                obstacles[i].lastSpawnGlobalDist = -1000f;
            }

            Prewarm();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void Update()
        {
            HandleSpawning();
            HandleCleanup();
        }

        private void HandleMovement()
        {
            float frameMove = speed * Time.fixedDeltaTime;

            // Track distance for spawn timing
            distanceTravelled += frameMove;
            globalDistanceCounter += frameMove;

            // Move objects using MovePosition to ensure character physics (ramps) work
            foreach (Rigidbody rb in activeObstacles)
            {
                if (rb != null)
                {
                    Vector3 newPos = rb.position + (direction * frameMove);
                    rb.MovePosition(newPos);
                }
            }
        }

        private void HandleSpawning()
        {
            // If we've moved far enough for a new potential spawn
            if (distanceTravelled >= density)
            {
                float overshoot = distanceTravelled - density;
                distanceTravelled = overshoot; // Reset with overshoot for mathematical precision

                // 1. Roll the dice: Does anything spawn at this tick?
                if (Random.value > globalProbability) return;

                // 2. Filter candidates based on your specific spacing rules
                var candidates = obstacles.Where(o =>
                    (globalDistanceCounter - lastSpawnGlobalDist) >= o.MinimumGap &&
                    (globalDistanceCounter - lastSpawnGlobalDist) >= currentRequiredPadding &&
                    (globalDistanceCounter - o.lastSpawnGlobalDist) >= o.MinimumSelfGap
                ).ToList();

                if (candidates.Count == 0) return;

                // 3. Weighted Random Selection based on Priority
                ObstacleSettings selected = GetWeightedObstacle(candidates);

                // 4. Update the global and self trackers
                for (int i = 0; i < obstacles.Length; i++)
                {
                    if (obstacles[i].Prefab == selected.Prefab)
                    {
                        obstacles[i].lastSpawnGlobalDist = globalDistanceCounter;
                        break;
                    }
                }

                lastSpawnGlobalDist = globalDistanceCounter;
                currentRequiredPadding = selected.NextObstaclePadding;

                // 5. Spawn at the precise position
                Vector3 spawnPos = spawnPosition + (direction * overshoot);
                SpawnObstacle(spawnPos, selected.Prefab);
            }
        }

        private ObstacleSettings GetWeightedObstacle(List<ObstacleSettings> candidates)
        {
            int totalWeight = candidates.Sum(c => c.Priority);
            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var c in candidates)
            {
                currentWeight += c.Priority;
                if (randomValue < currentWeight)
                    return c;
            }
            return candidates[0];
        }

        private void SpawnObstacle(Vector3 pos, GameObject prefab)
        {
            GameObject go = Instantiate(prefab, pos, Quaternion.identity);

            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = go.AddComponent<Rigidbody>();
            }

            // Must be kinematic so MovePosition works and player physics respect it
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            activeObstacles.Enqueue(rb);
        }

        private void HandleCleanup()
        {
            if (activeObstacles.Count == 0) return;

            Rigidbody oldest = activeObstacles.Peek();
            if (oldest == null)
            {
                activeObstacles.Dequeue();
                return;
            }

            // Detect if object has passed the deletion threshold
            bool passed = (direction.z < 0) ? (oldest.position.z <= deleteAtZ) : (oldest.position.z >= deleteAtZ);

            if (passed)
            {
                Destroy(activeObstacles.Dequeue().gameObject);
            }
        }

        private void Prewarm()
        {
            float totalDist = Vector3.Distance(startPosition, spawnPosition);
            int steps = Mathf.FloorToInt(totalDist / density);

            for (int i = steps; i > 0; i--)
            {
                // Calculate position along the track during startup
                Vector3 pos = spawnPosition + (-direction * (i * density));

                // For prewarm, we simplify logic but respect basic probability
                if (Random.value <= globalProbability)
                {
                    // For a true prewarm, you'd filter candidates here too, 
                    // but usually, a simple random pick is fine for game start.
                    GameObject prefab = obstacles[Random.Range(0, obstacles.Length)].Prefab;
                    SpawnObstacle(pos, prefab);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPosition, 0.5f);

            // Draw density markers
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            for (int i = 0; i < 5; i++)
            {
                Gizmos.DrawWireCube(spawnPosition + (-direction * (i * density)), new Vector3(laneWidth * 3, 1, 0.1f));
            }

            Gizmos.color = Color.red;
            Vector3 cleanupLine = new Vector3(spawnPosition.x, spawnPosition.y, deleteAtZ);
            Gizmos.DrawCube(cleanupLine, new Vector3(10f, 0.1f, 0.1f));
        }

        // Helper to visualize lane width in gizmos if needed
        private float laneWidth => LaneManager.LaneWidth;
    }
}