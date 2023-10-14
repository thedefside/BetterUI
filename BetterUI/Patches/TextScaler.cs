using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterUI.Patches
{
    internal class TextScaler : MonoBehaviour
    {
        public void Update()
        {
            // this sets the total (lossy) X scale to the same as the total lossy Y scale of the text component

            Vector3 prevScale = transform.localScale;

            float reverseParentTotalXScale = 1 / transform.parent.lossyScale.x;
            float parentTotalYScale = transform.parent.lossyScale.y;
            prevScale.x = reverseParentTotalXScale * parentTotalYScale * prevScale.y;

            this.transform.localScale = prevScale;
        }
    }
}
