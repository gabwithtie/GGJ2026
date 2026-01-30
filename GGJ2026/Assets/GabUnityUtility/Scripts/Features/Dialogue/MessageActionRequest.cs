
using External.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/ActionRequest/Message")]
    public class MessageActionRequest : ActionRequest
    {
        public CharacterSet CharacterSet;
        public Message message_;
    }
}
