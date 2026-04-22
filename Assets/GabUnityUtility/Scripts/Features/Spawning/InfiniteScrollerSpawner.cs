using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GabUnity
{
    public enum SpawnerMode { ScrollObjects, FollowReference }

    [System.Serializable]
    public struct ScrollerObjectData
    {
        public List<GameObject> prefabs;
        public int easy_minimum_space;
        public int hard_minimum_space;
        public int last_spawn;

        public int Get_minimumSpace(float diff)
        {
            return Mathf.RoundToInt(Mathf.Lerp(easy_minimum_space, hard_minimum_space, diff));
        }
    }

    public class InfiniteScrollerSpawner : MonoBehaviour
    {
        [Header("Mode Settings")]
        [SerializeField] private SpawnerMode mode = SpawnerMode.ScrollObjects;
        [SerializeField] private Transform referenceTransform; // The Player or Camera

        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> prewarm_list;
        [SerializeField] private List<ScrollerObjectData> objects;
        [SerializeField] private float width = 10.0f;
        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private float currentDiff;

        [Header("Movement Settings")]
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private bool use_global = true;
        private float Speed => _speed;
        [SerializeField] private Vector3 direction = Vector3.back;

        [Header("Cleanup Settings")]
        [SerializeField] private float lookBehindDistance = 20.0f; // Distance behind reference to delete

        private GameObject last_spawned;
        private Queue<GameObject> activeObjects = new Queue<GameObject>();
        private int prewarm_index = 0;
        private Vector3 internalSpawnCursor; // Tracks where the NEXT object should be in world space

        private void Start()
        {
            direction = direction.normalized;
            // Initialize the cursor at the starting point
            internalSpawnCursor = transform.TransformPoint(startPosition);
            Prewarm();
        }

        private void Update()
        {
            if (mode == SpawnerMode.ScrollObjects)
            {
                HandleMovement();
            }

            HandleSpawning();
            HandleCleanup();
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
            if (mode == SpawnerMode.FollowReference && referenceTransform != null) {
                float currentSqrDist = Vector3.SqrMagnitude(referenceTransform.position - last_spawned.transform.position);

                if (currentSqrDist < spawnPosition.z * spawnPosition.z)
                {
                    SpawnObject();
                }
            } else {
                Vector3 targetPoint = transform.TransformPoint(spawnPosition);

                float currentSqrDist = Vector3.SqrMagnitude(targetPoint - last_spawned.transform.position);

                if (currentSqrDist >= width * width)
                {
                    SpawnObject();
                }
            }
        }

        private void SpawnObject()
        {
            // Calculate the placement for the next tile
            // We use the last object's position and add 'width' in the OPPOSITE of 'direction'
            // (Since direction is usually 'back', we spawn 'forward')
            Vector3 spawnDir = -direction;

            Vector3 start = transform.position + startPosition;
            if (last_spawned != null)
                start = last_spawned.transform.position;

            Vector3 newSpawnPos = start + (spawnDir * width);

            GameObject nextPrefab = null;

            if (prewarm_index < prewarm_list.Count)
            {
                nextPrefab = prewarm_list[prewarm_index];
                prewarm_index++;
            }
            else
            {
                nextPrefab = GetNext();
            }

            if (nextPrefab != null)
            {
                last_spawned = Instantiate(nextPrefab, newSpawnPos, Quaternion.identity);
                activeObjects.Enqueue(last_spawned);
            }
        }

        private void HandleCleanup()
        {
            if (activeObjects.Count == 0) return;

            GameObject oldest = activeObjects.Peek();
            if (oldest == null) { activeObjects.Dequeue(); return; }

            bool shouldDelete = false;

            if (mode == SpawnerMode.FollowReference && referenceTransform != null)
            {
                // Delete based on distance from player
                float dist = Vector3.Dot(oldest.transform.position - referenceTransform.position, direction);
                shouldDelete = dist > lookBehindDistance;
            }
            else
            {
                // Original fixed-coordinate cleanup
                float deleteAtZ = transform.TransformPoint(new Vector3(0, 0, -lookBehindDistance)).z;
                shouldDelete = (direction.z < 0) ? (oldest.transform.position.z <= deleteAtZ) : (oldest.transform.position.z >= deleteAtZ);
            }

            if (shouldDelete)
            {
                Destroy(activeObjects.Dequeue());
            }
        }

        // ... GetNext() and Prewarm() logic remains same as your provided version ...
        private GameObject GetNext()
        {
            if (objects == null || objects.Count == 0) return null;
            for (int i = 0; i < objects.Count; i++)
            {
                var temp = objects[i];
                temp.last_spawn++;
                objects[i] = temp;
            }

            List<int> eligibleIndices = new List<int>();
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].last_spawn >= objects[i].Get_minimumSpace(currentDiff))
                    eligibleIndices.Add(i);
            }

            int selectedIndex = 0;
            if (eligibleIndices.Count > 0)
                selectedIndex = eligibleIndices[Random.Range(0, eligibleIndices.Count)];
            else
            {
                int longestWait = -1;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].last_spawn > longestWait)
                    {
                        longestWait = objects[i].last_spawn;
                        selectedIndex = i;
                    }
                }
            }

            var selectedData = objects[selectedIndex];
            selectedData.last_spawn = 0;
            objects[selectedIndex] = selectedData;

            return (selectedData.prefabs != null && selectedData.prefabs.Count > 0)
                    ? selectedData.prefabs[Random.Range(0, selectedData.prefabs.Count)]
                    : null;
        }

        private void Prewarm()
        {
            // Initial spawn at startPosition
            if (prewarm_list.Count > 0)
            {
                last_spawned = Instantiate(prewarm_list[0], transform.TransformPoint(startPosition), Quaternion.identity);
                activeObjects.Enqueue(last_spawned);
            }

            // Fill the gap
            float totalDist = Vector3.Distance(startPosition, spawnPosition);
            int count = Mathf.CeilToInt(totalDist / width);
            for (int i = 0; i < count; i++)
            {
                SpawnObject();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.TransformPoint(spawnPosition), new Vector3(width, 0.5f, 0.1f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.TransformPoint(startPosition), new Vector3(width, 0.5f, 0.1f));
        }
    }
}