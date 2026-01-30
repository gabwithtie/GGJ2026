using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class GenericActionRequestHandler : ActionRequestHandler_Base
    {
        [Serializable]
        struct GenericActionRequestEvent{
            public UnityEvent onRequest;
            public ActionRequest listening_for;
        }

        [SerializeField] private List<GenericActionRequestEvent> request_events = new();


        private void Awake()
        {
            ActionRequestManager.RegisterHandler(request_events[0].listening_for.GetType(), this);
        }

        private void OnDestroy()
        {
            ActionRequestManager.UnregisterHandler(request_events[0].listening_for.GetType(), this);
        }

        public override bool Request(ActionRequest somerequest)
        {
            bool atleastone = false;

            foreach(var request_event in request_events)
            {
                if(somerequest == request_event.listening_for)
                {
                    request_event.onRequest.Invoke();
                    atleastone = true;
                }
            }

            return atleastone;
        }
    }
}
