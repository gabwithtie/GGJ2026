using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabUnity
{
    public abstract class ActionRequestHandler_Base : MonoBehaviour
    {
        
        abstract public bool Request(ActionRequest somerequest);
    }

    public abstract class ActionRequestHandler<T>: ActionRequestHandler_Base where T : ActionRequest
    {
        private void Awake()
        {
            ActionRequestManager.RegisterHandler(typeof(T), this);
        }

        public override bool Request(ActionRequest somerequest)
        {
            if (somerequest.GetType() == typeof(T))
            {
                return this.ProcessRequest((T)somerequest);
            }
            else
                return false;
        }

        protected abstract bool ProcessRequest(T somerequest);
    }
}
