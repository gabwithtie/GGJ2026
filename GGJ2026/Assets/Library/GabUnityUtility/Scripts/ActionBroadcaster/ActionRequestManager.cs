using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabUnity
{
    public class ActionRequestManager: MonoSingleton<ActionRequestManager>
    {
        private readonly Dictionary<System.Type, List<ActionRequestHandler_Base>> handler_list = new();

        [SerializeField] private bool printEvents = false;

        public static void RegisterHandler(System.Type handleType, ActionRequestHandler_Base handler)
        {
            if (Instance.handler_list.ContainsKey(handleType))
            {
                Instance.handler_list[handleType].Add(handler);
                return;
            }

            Instance.handler_list.Add(handleType, new());
            Instance.handler_list[handleType].Add(handler);

            Debug.Log("Registered Handler: " + handler.name + " of type: \"" + handleType.ToString() + "\"");
        }

        public static void UnregisterHandler(System.Type handleType, ActionRequestHandler_Base handler)
        {
            if (Instance.handler_list.ContainsKey(handleType))
            {
                Instance.handler_list[handleType].Remove(handler);
            }
        }

        public static void Request(ActionRequest request)
        {
            var requestType = request.GetType();
            if (Instance.handler_list.ContainsKey(requestType))
            {
                if(Instance.printEvents)
                    UnityEngine.Debug.Log($"ActionRequestManager: Requesting {request.name}");

                foreach (var handler in Instance.handler_list[requestType])
                {
                    var response = handler.Request(request);

                    if (Instance.printEvents)
                        UnityEngine.Debug.Log($"ActionRequestManager: {handler.name} response: {response}");
                }
            }
        }
    }
}
