using System.Collections.Generic;
using UnityEngine;

namespace External.Dialogue {
    [System.Serializable]
    public class Message
    {
        public enum Align { left, right }

        [Header("Data")]
        public Align align;

        [Header("Text")]
        public int name_index;
        [TextArea] public string text;
        [Header("Art")]
        public int art_variant;
    }
}
