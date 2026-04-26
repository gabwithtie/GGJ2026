using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{

    public class DoOnTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;

        private void OnTriggerEnter(Collider other)
        {
            onEnter.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            onExit.Invoke();
        }
    }
}