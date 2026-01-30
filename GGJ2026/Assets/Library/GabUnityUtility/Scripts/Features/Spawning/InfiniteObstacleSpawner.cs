using UnityEngine;
using System.Collections.Generic;

namespace GabUnity
{
    public class InfiniteObstacleSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private float density = 5.0f; // Distance between spawn checks
        [Range(0f, 1f)][SerializeField] private float probability = 0.5f; // Chance to spawn (0% to 100%)

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
        private Queue<GameObject> activeObstacles = new Queue<GameObject>();

        private void Start()
        {
            direction = direction.normalized;
            Prewarm();
        }

        private void Update()
        {
            HandleMovement();
            HandleSpawning();
            HandleCleanup();
        }

        private void Prewarm()
        {
            float totalDist = Vector3.Distance(startPosition, spawnPosition);
            int steps = Mathf.FloorToInt(totalDist / density);

            for (int i = 0; i < steps; i++)
            {
                // Calculate position behind the spawn point
                Vector3 pos = spawnPosition + (-direction * (i * density));

                // Roll the dice for prewarm
                if (Random.value <= probability)
                {
                    SpawnObstacle(pos);
                }
            }
        }

        private void HandleMovement()
        {
            // Track distance based on speed and time
            distanceTravelled += speed * Time.deltaTime;

            foreach (GameObject obj in activeObstacles)
            {
                if (obj != null) obj.transform.position += direction * speed * Time.deltaTime;
            }
        }

        private void HandleSpawning()
        {
            // If we've moved a distance equal to our density setting
            if (distanceTravelled >= density)
            {
                // Calculate overshoot to keep it mathematically accurate
                float overshoot = distanceTravelled - density;
                Vector3 precisePos = spawnPosition + (direction * overshoot);

                // Roll the dice
                if (Random.value <= probability)
                {
                    SpawnObstacle(precisePos);
                }

                // Reset tracker but keep the overshoot for accuracy
                distanceTravelled = overshoot;
            }
        }

        private void SpawnObstacle(Vector3 pos)
        {
            if (obstaclePrefabs.Length == 0) return;

            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject go = Instantiate(prefab, pos, Quaternion.identity);
            activeObstacles.Enqueue(go);
        }

        private void HandleCleanup()
        {
            if (activeObstacles.Count == 0) return;

            GameObject oldest = activeObstacles.Peek();
            if (oldest == null) { activeObstacles.Dequeue(); return; }

            bool passed = (direction.z < 0) ? (oldest.transform.position.z <= deleteAtZ) : (oldest.transform.position.z >= deleteAtZ);

            if (passed)
            {
                Destroy(activeObstacles.Dequeue());
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPosition, 0.5f);

            // Draw density markers to visualize the rhythm
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            for (int i = 0; i < 5; i++)
            {
                Gizmos.DrawWireCube(spawnPosition + (-direction * (i * density)), new Vector3(1, 1, 0.1f));
            }

            Gizmos.color = Color.red;
            Vector3 cleanupLine = new Vector3(spawnPosition.x, spawnPosition.y, deleteAtZ);
            Gizmos.DrawCube(cleanupLine, new Vector3(5f, 0.1f, 0.1f));
        }
    }
}