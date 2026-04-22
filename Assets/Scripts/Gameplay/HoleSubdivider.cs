using UnityEngine;

namespace GabUnity
{
    public class HoleSubdivider : MonoBehaviour
    {
        [Tooltip("The scaled unit cube that represents this object's physical volume.")]
        public Transform childUnitCube;

        private bool _hasBeenSubdivided = false;

        public void CutHole(Vector3 worldPointA, Vector3 worldPointB)
        {
            if (_hasBeenSubdivided) return;
            if (childUnitCube == null)
            {
                Debug.LogWarning("No child unit cube assigned to HoleSubdivider.");
                return;
            }

            Vector3 localA = transform.InverseTransformPoint(worldPointA);
            Vector3 localB = transform.InverseTransformPoint(worldPointB);

            // Determine bounds of the hole on the XY plane
            float holeMinX = Mathf.Min(localA.x, localB.x);
            float holeMaxX = Mathf.Max(localA.x, localB.x);
            float holeMinY = Mathf.Min(localA.y, localB.y);
            float holeMaxY = Mathf.Max(localA.y, localB.y);

            // Get bounds of the original scaled unit cube
            Vector3 scale = childUnitCube.localScale;
            float extentsX = scale.x / 2f;
            float extentsY = scale.y / 2f;

            // Clamp hole bounds so we don't try to cut outside the wall
            holeMinX = Mathf.Clamp(holeMinX, -extentsX, extentsX);
            holeMaxX = Mathf.Clamp(holeMaxX, -extentsX, extentsX);
            holeMinY = Mathf.Clamp(holeMinY, -extentsY, extentsY);
            holeMaxY = Mathf.Clamp(holeMaxY, -extentsY, extentsY);

            if (holeMinX == holeMaxX || holeMinY == holeMaxY) return;

            // Create the 4 sub-cubes that frame the hole along the XY plane

            // 1. Left Block (Full height, left of the hole)
            CreateSubCube("Cube_Left",
                new Vector3((-extentsX + holeMinX) / 2f, 0, 0),
                new Vector3(holeMinX - (-extentsX), scale.y, scale.z));

            // 2. Right Block (Full height, right of the hole)
            CreateSubCube("Cube_Right",
                new Vector3((holeMaxX + extentsX) / 2f, 0, 0),
                new Vector3(extentsX - holeMaxX, scale.y, scale.z));

            // 3. Top Block (Directly above the hole, between left/right blocks)
            CreateSubCube("Cube_Top",
                new Vector3((holeMinX + holeMaxX) / 2f, (holeMaxY + extentsY) / 2f, 0),
                new Vector3(holeMaxX - holeMinX, extentsY - holeMaxY, scale.z));

            // 4. Bottom Block (Directly below the hole, between left/right blocks)
            CreateSubCube("Cube_Bottom",
                new Vector3((holeMinX + holeMaxX) / 2f, (-extentsY + holeMinY) / 2f, 0),
                new Vector3(holeMaxX - holeMinX, holeMinY - (-extentsY), scale.z));

            _hasBeenSubdivided = true;
            childUnitCube.gameObject.SetActive(false);
        }

        private void CreateSubCube(string cubeName, Vector3 localPos, Vector3 localScale)
        {
            // Abort if the slice thickness on either the X or Y axis is virtually zero
            if (localScale.x <= 0.001f || localScale.y <= 0.001f) return;

            GameObject newCube = Instantiate(childUnitCube.gameObject, transform);
            newCube.name = cubeName;
            newCube.transform.localPosition = localPos;
            newCube.transform.localScale = localScale;
            newCube.transform.localRotation = Quaternion.identity;

            newCube.SetActive(true);
        }
    }
}