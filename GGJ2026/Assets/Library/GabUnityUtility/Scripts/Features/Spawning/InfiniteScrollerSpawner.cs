using UnityEngine;
using System.Collections.Generic;

namespace GabUnity
{
    [System.Serializable]
    public struct ScrollerObjectData
    {
        public string name; // Added for easier debugging in inspector
        public GameObject prefab;
        [Range(0f, 1f)] public float skipProbability; // 0 = never skip, 1 = always skip
    }

    public class InfiniteScrollerSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> prewarm_list;
        [SerializeField] private List<ScrollerObjectData> objects;
        [SerializeField] private float width = 10.0f;
        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private Vector3 startPosition;

        [Header("Movement Settings")]
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private bool use_global = true;
        private float Speed => use_global ? InfiniteScrollerManager.ScrollSpeed : _speed;
        [SerializeField] private Vector3 direction = Vector3.back;

        [Header("Cleanup Settings")]
        [SerializeField] private float deleteAtZ = -20.0f;


        private GameObject last_spawned;
        private Queue<GameObject> activeObjects = new Queue<GameObject>();
        private int cur_object_index = -1;

        private int prewarm_index = 0;

        /// <summary>
        /// Selects the next object, but skips to the next index if the probability roll fails.
        /// </summary>
        private ScrollerObjectData GetNextObjectData()
        {
            if (objects.Count == 0) return default;

            while (true)
            {
                cur_object_index = (cur_object_index + 1) % objects.Count;
                var candidate = objects[cur_object_index];

                // Roll for skip. 
                // If we roll a value higher than the probability, we keep it.
                // Otherwise, the loop continues to the next index.
                if (Random.value >= candidate.skipProbability)
                {
                    return candidate;
                }

                // Safety: if you accidentally set ALL objects to 1.0 skipProbability, 
                // this loop would hang Unity. In a real project, add a counter break here.
            }
        }

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
            int count = Mathf.CeilToInt(totalDist / width);

            // Spawn the very first piece at the start position
            var data = GetNextObjectData();
            last_spawned = Instantiate(data.prefab, startPosition, Quaternion.identity);
            activeObjects.Enqueue(last_spawned);

            // Fill the rest up to the spawn point
            for (int i = 0; i < count; i++)
            {
                SpawnObject();
            }
        }

        private void HandleMovement()
        {
            float frameMove = Speed * Time.deltaTime;
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null) obj.transform.position += direction * frameMove;
            }
        }

        private void HandleSpawning()
        {
            if (last_spawned == null) return;

            float currentSqrDist = Vector3.SqrMagnitude(spawnPosition - last_spawned.transform.position);

            if (currentSqrDist >= width * width)
            {
                SpawnObject();
            }
        }

        private void SpawnObject()
        {
            // Use the position of the last spawned object to ensure no gaps
            float currentDist = Vector3.Distance(spawnPosition, last_spawned.transform.position);
            float overshootDist = currentDist - width;
            Vector3 newSpawnPos = spawnPosition + (direction * overshootDist);

            if (prewarm_index < prewarm_list.Count)
            {
                last_spawned = Instantiate(prewarm_list[prewarm_index], newSpawnPos, Quaternion.identity);
                activeObjects.Enqueue(last_spawned);

                prewarm_index++;
            }
            else
            {
                var data = GetNextObjectData();
                last_spawned = Instantiate(data.prefab, newSpawnPos, Quaternion.identity);
                activeObjects.Enqueue(last_spawned);
            }
        }

        private void HandleCleanup()
        {
            if (activeObjects.Count == 0) return;

            GameObject oldest = activeObjects.Peek();
            if (oldest == null) { activeObjects.Dequeue(); return; }

            bool passed = (direction.z < 0) ? (oldest.transform.position.z <= deleteAtZ) : (oldest.transform.position.z >= deleteAtZ);

            if (passed)
            {
                Destroy(activeObjects.Dequeue());
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnPosition, new Vector3(width, 0.5f, 0.1f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(startPosition, new Vector3(width, 0.5f, 0.1f));
        }
    }
}