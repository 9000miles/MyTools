using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    public class VariablePopupAttribute : PropertyAttribute
    {

        public bool showReferenceDatabase = false;

        public VariablePopupAttribute(bool showReferenceDatabase = false)
        {
            this.showReferenceDatabase = showReferenceDatabase;
        }
    }
}
