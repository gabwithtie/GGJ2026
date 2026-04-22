using System.Collections.Generic;
using UnityEngine;

namespace External.Dialogue
{
    [System.Serializable]
    public class Dialogue
    {
        [Header("Settings")]
        public bool manually_triggered = false;

        [Header("Messages")]
        public List<Message> Messages;
    }
}