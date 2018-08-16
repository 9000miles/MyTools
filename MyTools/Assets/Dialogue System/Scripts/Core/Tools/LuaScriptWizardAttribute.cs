using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    public class LuaScriptWizardAttribute : PropertyAttribute
    {

        public bool showReferenceDatabase = false;

        public LuaScriptWizardAttribute(bool showReferenceDatabase = false)
        {
            this.showReferenceDatabase = showReferenceDatabase;
        }
    }
}
