using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.UI
{
    class UIInteractable : UIContainer
    {
        public UIInteractable(float x, float y, float width, float height, UIContainer parent = null, UISizeMethod sizing = UISizeMethod.UV) : base(x, y, width, height, parent, sizing)
        {

        }
    }
}
