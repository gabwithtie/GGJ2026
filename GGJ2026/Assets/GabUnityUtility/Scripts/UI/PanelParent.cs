using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GabUnity
{
    public class PanelParent : Panel
    {
        private List<Panel> childPanels = new List<Panel>();

        private void Awake()
        {
            this.gameObject.GetComponentsInChildren(childPanels);
        }

        private void Start()
        {
            SetOpen(isOpen);
        }

        public override void SetOpen(bool open)
        {
            base.SetOpen(open);

            foreach (Panel panel in childPanels)
            {
                if (panel == this)
                    continue;

                panel.SetOpen(open);
            }
        }
    }
}
