using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;

namespace GabUnity
{
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
        private float Speed => use_global ? InfiniteScrollerManager.ScrollSpeed : _speed;
        [SerializeField] private Vector3 direction = Vector3.back;

        [Header("Cleanup Settings")]
        [SerializeField] private float deleteAtZ = -20.0f;

        private GameObject last_spawned;
        private Queue<GameObject> activeObjects = new Queue<GameObject>();
        private int prewarm_index = 0;

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

        private GameObject GetNext()
        {
            if (objects == null || objects.Count == 0) return null;


            // 2. Increment 'last_spawn' for all groups to track distance
            for (int i = 0; i < objects.Count; i++)
            {
                var temp = objects[i];
                temp.last_spawn++;
                objects[i] = temp;
            }

            // 3. Find all groups that have waited long enough
            List<int> eligibleIndices = new List<int>();
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].last_spawn >= objects[i].Get_minimumSpace(currentDiff))
                {
                    eligibleIndices.Add(i);
                }
            }

            int selectedIndex = 0;

            // 4. Select a group
            if (eligibleIndices.Count > 0)
            {
                // Pick a random group from the eligible pool
                selectedIndex = eligibleIndices[Random.Range(0, eligibleIndices.Count)];
            }
            else
            {
                // Fallback: If no groups are ready, pick the one that has been waiting the longest
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

            // 5. Reset the counter for the selected group
            var selectedData = objects[selectedIndex];
            selectedData.last_spawn = 0;
            objects[selectedIndex] = selectedData;

            // 6. Return a random prefab from that specific group
            if (selectedData.prefabs != null && selectedData.prefabs.Count > 0)
            {
                return selectedData.prefabs[Random.Range(0, selectedData.prefabs.Count)];
            }

            return null;
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
            float currentDist = 0;
            if (last_spawned != null)
                currentDist = Vector3.Distance(spawnPosition, last_spawned.transform.position);

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
                var data = GetNext();
                last_spawned = Instantiate(data, newSpawnPos, Quaternion.identity);
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