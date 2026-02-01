using UnityEngine;
using System.Collections.Generic;

namespace GabUnity
{
    public class InfiniteScrollerSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> objects;
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

        private int cur_object = 0;

        private GameObject GetObject() {
            cur_object++;
            cur_object %= objects.Count;
            return objects[cur_object];
        }

        private void Start()
        {
            // Ensure direction is a unit vector for predictable math
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

            // 1. Spawn the "Head" (the leading edge)
            SpawnObject(spawnPosition);

            // 2. Fill BACKWARDS from the spawn point towards the start point
            // We use -direction to place objects behind the head
            for (int i = 0; i <= count; i++)
            {
                Vector3 pos = startPosition + (-direction * (i * width));
                GameObject go = Instantiate(GetObject(), pos, Quaternion.identity);
                activeObjects.Enqueue(go);
            }
        }

        private void HandleMovement()
        {
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null) obj.transform.position += direction * Speed * Time.deltaTime;
            }
        }

        private void HandleSpawning()
        {
            if (last_spawned == null) return;

            // Calculate current distance from spawn point
            float currentDist = Vector3.Distance(spawnPosition, last_spawned.transform.position);

            if (currentDist >= width)
            {
                // SEAMLESS CORRECTION:
                // Find exactly how much we overshot 'width' this frame
                float overshootDist = currentDist - width;

                // Place the new object exactly 'width' units away from the last one, 
                // but accounting for the movement that happened this frame.
                Vector3 newSpawnPos = spawnPosition + (direction * overshootDist);

                SpawnObject(newSpawnPos);
            }
        }

        private void SpawnObject(Vector3 pos)
        {
            last_spawned = Instantiate(GetObject(), pos, Quaternion.identity);
            activeObjects.Enqueue(last_spawned);
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
            Gizmos.DrawWireCube(spawnPosition, new Vector3(width, 1f, 0.1f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(startPosition, new Vector3(width, 1f, 0.1f));

            // Show the "Prewarm Area"
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawLine(spawnPosition, startPosition);
        }
    }
}