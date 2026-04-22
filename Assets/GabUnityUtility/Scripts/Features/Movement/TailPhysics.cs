using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace GabUnity
{
    public class TailPhysics : MonoBehaviour
    {
        public enum AlignmentAxis { Forward, Up, Right }

        [Header("Setup")]
        public Transform TailRoot;
        public AlignmentAxis JoinAxis = AlignmentAxis.Forward;

        [Header("Physics Settings")]
        public float GravityScale = 1.0f;
        [Range(0, 1)] public float Stiffness = 0.1f;
        [Range(0, 1)] public float Drag = 0.1f;
        public Vector3 GravityDirection = Vector3.down;

        [Header("Twitch Settings")]
        public bool DoesTwitch = false;
        [Tooltip("Frequency of twitches (Min/Max interval in seconds)")]
        public Vector2 TwitchInterval = new Vector2(1.0f, 5.0f);
        [Tooltip("How violent the twitch is")]
        public float TwitchIntensity = 0.5f;

        private List<TailSegment> _segments = new List<TailSegment>();
        private List<float> _segmentDistances = new List<float>();
        private float _nextTwitchTime;

        private struct TailSegment
        {
            public Transform Transform;
            public Vector3 CurrentPosition;
            public Vector3 PreviousPosition;

            public TailSegment(Transform t)
            {
                Transform = t;
                CurrentPosition = t.position;
                PreviousPosition = t.position;
            }
        }

        private void Start()
        {
            if (TailRoot == null) TailRoot = transform;

            Transform current = TailRoot;
            while (current != null)
            {
                _segments.Add(new TailSegment(current));

                if (current.childCount > 0)
                {
                    Transform child = current.GetChild(0);
                    _segmentDistances.Add(Vector3.Distance(current.position, child.position));
                    current = child;
                }
                else { current = null; }
            }

            SetNextTwitchTime();
        }

        private void SetNextTwitchTime()
        {
            _nextTwitchTime = Time.time + Random.Range(TwitchInterval.x, TwitchInterval.y);
        }

        private void FixedUpdate()
        {
            if (_segments.Count == 0) return;

            // --- TWITCH LOGIC ---
            Vector3 twitchOffset = Vector3.zero;
            if (DoesTwitch && Time.time >= _nextTwitchTime)
            {
                // Create a random directional pop
                twitchOffset = Random.onUnitSphere * TwitchIntensity;
                SetNextTwitchTime();
            }

            // 1. Update the Root
            var root = _segments[0];
            root.CurrentPosition = _segments[0].Transform.position;
            root.PreviousPosition = root.CurrentPosition;
            _segments[0] = root;

            // 2. Verlet Integration
            for (int i = 1; i < _segments.Count; i++)
            {
                var seg = _segments[i];

                Vector3 velocity = (seg.CurrentPosition - seg.PreviousPosition) * (1f - Drag);
                seg.PreviousPosition = seg.CurrentPosition;

                seg.CurrentPosition += velocity;
                seg.CurrentPosition += GravityDirection * GravityScale * Time.fixedDeltaTime;

                // Apply the twitch to non-root segments
                if (twitchOffset != Vector3.zero)
                {
                    // Multiply twitch by index so the tip of the tail twitches more than the base
                    seg.CurrentPosition += twitchOffset * (i / (float)_segments.Count);
                }

                // Constraint
                Vector3 targetPos = _segments[i - 1].CurrentPosition;
                float dist = _segmentDistances[i - 1];
                Vector3 direction = (seg.CurrentPosition - targetPos).normalized;
                seg.CurrentPosition = targetPos + direction * dist;

                _segments[i] = seg;
            }

            // 3. Apply Visuals
            // Inside the Final Pass loop of FixedUpdate()
            for (int i = 0; i < _segments.Count; i++)
            {
                _segments[i].Transform.position = _segments[i].CurrentPosition;

                if (i > 0 && i < _segments.Count - 1)
                {
                    Vector3 lookDir = _segments[i + 1].CurrentPosition - _segments[i].CurrentPosition;

                    if (lookDir.sqrMagnitude > 0.0001f)
                    {
                        // Pass 'i-1' (the parent) as the reference instead of '0' (the root)
                        ApplyStabilizedRotation(_segments[i].Transform, lookDir, _segments[i - 1].Transform);
                    }
                }
            }
        }

        // Replace your ApplyStabilizedRotation with this logic
        private void ApplyStabilizedRotation(Transform t, Vector3 direction, Transform parentTransform)
        {
            // Use the IMMEDIATE parent's orientation instead of the Root
            // This creates a smooth 'twist' transition down the tail instead of vibrating
            Vector3 referenceUp = parentTransform.up;

            Quaternion targetRot = Quaternion.identity;

            switch (JoinAxis)
            {
                case AlignmentAxis.Forward:
                    // If direction and up are parallel, Quaternion.LookRotation fails/jitters
                    // We check for that 'Gimbal Lock' state
                    if (Mathf.Abs(Vector3.Dot(direction.normalized, referenceUp.normalized)) > 0.99f)
                    {
                        referenceUp = parentTransform.forward; // Switch reference if pointing straight up
                    }
                    targetRot = Quaternion.LookRotation(direction, referenceUp);
                    break;

                case AlignmentAxis.Up:
                    targetRot = Quaternion.FromToRotation(Vector3.up, direction);
                    // Re-align the secondary axis to match parent
                    Vector3 projectedForward = Vector3.ProjectOnPlane(parentTransform.forward, direction);
                    if (projectedForward.sqrMagnitude > 0.01f)
                    {
                        targetRot = Quaternion.LookRotation(projectedForward, direction);
                    }
                    break;

                case AlignmentAxis.Right:
                    targetRot = Quaternion.FromToRotation(Vector3.right, direction);
                    Vector3 projectedUp = Vector3.ProjectOnPlane(parentTransform.up, direction);
                    if (projectedUp.sqrMagnitude > 0.01f)
                    {
                        targetRot = Quaternion.LookRotation(direction, projectedUp);
                        targetRot *= Quaternion.Euler(0, -90, 0); // Correct for Right-axis offset
                    }
                    break;
            }

            // LOWER the rotation speed. 
            // High speed + physics constraints = Vibration.
            // 10f - 15f is usually the 'sweet spot' for smooth tails.
            float lerpSpeed = Stiffness * 15f;
            t.rotation = Quaternion.Slerp(t.rotation, targetRot, lerpSpeed * Time.fixedDeltaTime);
        }
    }
}