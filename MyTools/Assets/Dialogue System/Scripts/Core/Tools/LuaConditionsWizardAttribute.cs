using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    public class LuaConditionsWizardAttribute : PropertyAttribute
    {

        public bool showReferenceDatabase = false;

        public LuaConditionsWizardAttribute(bool showReferenceDatabase = false)
        {
            this.showReferenceDatabase = showReferenceDatabase;
        }
    }
}
