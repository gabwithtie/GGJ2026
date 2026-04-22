
using External.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/ActionRequest/Dialogue")]
    public class DialogueActionRequest : ActionRequest
    {
        public CharacterSet characterset;
        public Dialogue dialogue;
    }
}
