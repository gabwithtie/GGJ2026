// 12/26/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GabUnity
{
    public class HealthBarManager : MonoBehaviour
    {
        [Header("Health Bar Settings")]
        [SerializeField] private GameObject healthBarPrefab; // Prefab for the health bar UI
        [SerializeField] private int maxHealthBars = 10; // Maximum number of health bars in the pool

        private List<Image> healthBarPool = new List<Image>();
        private List<HealthObject> hpobjects = new List<HealthObject>();

        private void Start()
        {
            // Initialize the health bar pool
            for (int i = 0; i < maxHealthBars; i++)
            {
                GameObject healthBarObject = Instantiate(healthBarPrefab, this.transform);
                healthBarObject.SetActive(false);
                healthBarPool.Add(healthBarObject.GetComponent<Image>());
            }
        }

        private void Update()
        {
            UpdateHealthBars();
        }

        public void RegisterUnit(UnitIdentifier unit)
        {
            HealthObject hp = null;

            if (unit.TryGetComponent(out hp) && !hpobjects.Contains(hp))
            {
                hpobjects.Add(hp);
            }
        }

        public void UnregisterUnit(UnitIdentifier unit)
        {
            HealthObject hp = null;

            if (unit.TryGetComponent(out hp) && hpobjects.Contains(hp))
            {
                hpobjects.Remove(hp);
            }
        }

        private void UpdateHealthBars()
        {
            // Sort units by distance to the center of the screen
            hpobjects.RemoveAll(unit => unit == null); // Clean up null references

            hpobjects.Sort((a, b) =>
            {
                float distanceA = Vector2.SqrMagnitude((Vector2)Camera.main.WorldToScreenPoint(a.transform.position) - new Vector2(Screen.width / 2, Screen.height / 2));
                float distanceB = Vector2.SqrMagnitude((Vector2)Camera.main.WorldToScreenPoint(b.transform.position) - new Vector2(Screen.width / 2, Screen.height / 2));
                return distanceA.CompareTo(distanceB);
            });

            // Assign health bars to the closest units
            for (int i = 0; i < healthBarPool.Count; i++)
            {
                if (i < hpobjects.Count)
                {
                    healthBarPool[i].gameObject.SetActive(true);
                    UpdateHealthBar(healthBarPool[i], hpobjects[i]);
                }
                else
                {
                    healthBarPool[i].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateHealthBar(Image healthBar, HealthObject unit)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);
            healthBar.transform.position = screenPosition;
            healthBar.fillAmount = unit.Health / unit.MaxHealth;
        }
    }
}