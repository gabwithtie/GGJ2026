using External.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabUnity
{
    public class MessageRequestHandler : ActionRequestHandler<MessageActionRequest>
    {
        protected override bool ProcessRequest(MessageActionRequest somerequest)
        {
            return DialogueController.LoadMessage(somerequest.message_, somerequest.CharacterSet);
        }
    }
}
