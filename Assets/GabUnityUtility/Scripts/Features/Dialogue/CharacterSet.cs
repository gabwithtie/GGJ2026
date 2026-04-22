using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static External.Dialogue.Dialogue;

namespace External.Dialogue
{
    [CreateAssetMenu(menuName = "GabUnity/Dialogue/CharacterSet")]
    public class CharacterSet : ScriptableObject
    {
        [System.Serializable]
        public struct DialogueSprite
        {
            public string id;
            public List<Sprite> images;
        }

        public List<DialogueSprite> characters;
    }
}
