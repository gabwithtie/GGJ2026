using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GabUnity
{
    [System.Serializable]
    public struct ScrollerObjectData
    {
        public string name;
        public GameObject prefab;
        [Tooltip("0 = Easy, 1 = Medium, 2 = Hard")]
        public int difficultyRating; //
        [Tooltip("Relative chance to spawn compared to others in the same difficulty pool")]
        public float baseWeight;
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

        [Header("Difficulty Scaling")]
        [SerializeField] private int hardCooldownSegments = 3; //
        private int _currentHardCooldown = 0; //

        private GameObject last_spawned;
        private Queue<GameObject> activeObjects = new Queue<GameObject>();
        private int prewarm_index = 0;

        /// <summary>
        /// Selects the next object based on weighted probability and current difficulty ceiling.
        /// </summary>
        private ScrollerObjectData GetNextObjectWeighted()
        {
            if (objects.Count == 0) return default;

            // 1. Determine max allowed difficulty based on current speed
            int maxDifficulty = 0;
            if (Speed > 15f) maxDifficulty = 2;      // Hard segments unlocked
            else if (Speed > 8f) maxDifficulty = 1;   // Medium segments unlocked

            // 2. Filter eligible objects based on difficulty and hard-cooldown
            var eligible = new List<ScrollerObjectData>();
            foreach (var obj in objects)
            {
                if (obj.difficultyRating <= maxDifficulty)
                {
                    // Skip hard objects if we are currently in a cooldown period
                    if (obj.difficultyRating == 2 && _currentHardCooldown > 0) continue;
                    eligible.Add(obj);
                }
            }

            // Fallback if filtering left us empty
            if (eligible.Count == 0) eligible.Add(objects[0]);

            // 3. Weighted Random Selection
            float totalWeight = eligible.Sum(x => x.baseWeight);
            float roll = Random.Range(0, totalWeight);
            float cursor = 0;

            foreach (var candidate in eligible)
            {
                cursor += candidate.baseWeight;
                if (cursor >= roll)
                {
                    // 4. Update Hard Object Cooldown
                    if (candidate.difficultyRating == 2)
                        _currentHardCooldown = hardCooldownSegments;
                    else if (_currentHardCooldown > 0)
                        _currentHardCooldown--;

                    return candidate;
                }
            }

            return eligible[0];
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
            if(prewarm_list.Count > 0)
            {
                var data = prewarm_list[0];
                last_spawned = Instantiate(data, startPosition, Quaternion.identity);
                activeObjects.Enqueue(last_spawned);
            }

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
                // Use the new weighted selection instead of simple indexing
                var data = GetNextObjectWeighted();
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