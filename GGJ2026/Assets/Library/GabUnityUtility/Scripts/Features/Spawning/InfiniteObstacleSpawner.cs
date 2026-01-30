using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    public class InfiniteObstacleSpawner : MonoBehaviour
    {
        // ... Keep existing SerializedFields ...
        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private float density = 5.0f;
        [Range(0f, 1f)][SerializeField] private float probability = 0.5f;

        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private Vector3 startPosition;

        [Header("Movement Settings")]
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private bool use_global = true;
        private float speed => use_global ? InfiniteScrollerManager.ScrollSpeed : _speed;

        [SerializeField] private Vector3 direction = Vector3.back;

        [Header("Cleanup Settings")]
        [SerializeField] private float deleteAtZ = -20.0f;

        private float distanceTravelled;
        // CHANGE: Store Rigidbodies instead of GameObjects
        private Queue<Rigidbody> activeObstacles = new Queue<Rigidbody>();

        private void Start()
        {
            direction = direction.normalized;
            Prewarm();
        }

        // Logic split: Movement belongs in FixedUpdate for physics stability
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
            distanceTravelled += speed * Time.fixedDeltaTime;

            foreach (Rigidbody rb in activeObstacles)
            {
                if (rb != null)
                {
                    // MovePosition tells the physics engine the object moved from A to B
                    // allowing it to push other colliders (your player) out of the way.
                    Vector3 newPos = rb.position + (direction * speed * Time.fixedDeltaTime);
                    rb.MovePosition(newPos);
                }
            }
        }

        private void SpawnObstacle(Vector3 pos)
        {
            if (obstaclePrefabs.Length == 0) return;

            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject go = Instantiate(prefab, pos, Quaternion.identity);

            // Ensure the spawned object has a Rigidbody
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = go.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            activeObstacles.Enqueue(rb);
        }

        // ... Keep Prewarm, HandleSpawning, and HandleCleanup logic (just update types to Rigidbody) ...

        private void HandleCleanup()
        {
            if (activeObstacles.Count == 0) return;

            Rigidbody oldest = activeObstacles.Peek();
            if (oldest == null) { activeObstacles.Dequeue(); return; }

            bool passed = (direction.z < 0) ? (oldest.position.z <= deleteAtZ) : (oldest.position.z >= deleteAtZ);

            if (passed)
            {
                Destroy(activeObstacles.Dequeue().gameObject);
            }
        }

        // ... Keep Prewarm and OnDrawGizmos ...
        private void Prewarm()
        {
            float totalDist = Vector3.Distance(startPosition, spawnPosition);
            int steps = Mathf.FloorToInt(totalDist / density);

            for (int i = 0; i < steps; i++)
            {
                Vector3 pos = spawnPosition + (-direction * (i * density));
                if (Random.value <= probability)
                {
                    SpawnObstacle(pos);
                }
            }
        }

        private void HandleSpawning()
        {
            // Use Time.deltaTime here as it's called in Update
            float frameDist = speed * Time.deltaTime;
            // Note: We don't add to distanceTravelled here because HandleMovement handles it in FixedUpdate
            // However, to keep spawning synced with visual movement:
            if (distanceTravelled >= density)
            {
                float overshoot = distanceTravelled - density;
                Vector3 precisePos = spawnPosition + (direction * overshoot);

                if (Random.value <= probability)
                {
                    SpawnObstacle(precisePos);
                }
                distanceTravelled = overshoot;
            }
        }
    }
}