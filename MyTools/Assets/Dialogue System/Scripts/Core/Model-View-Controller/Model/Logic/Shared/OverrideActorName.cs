using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This component allows you to override the actor name used in conversations,
    /// which is normally set to the name of the GameObject. If the override name
    /// contains a [lua] or [var] tag, it parses the value.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/override_actor_name.html")]
#endif
    [AddComponentMenu("Dialogue System/Actor/Override Actor Name")]
    public class OverrideActorName : MonoBehaviour
    {

        /// <summary>
        /// Overrides the actor name used in conversations.
        /// </summary>
        [Tooltip("Use this actor name in conversations.")]
        public string overrideName;

        /// <summary>
        /// The internal name to use in the dialogue database when saving persistent data.
        /// If blank, uses the override name.
        /// </summary>
        [Tooltip("Name used when saving persistent data.")]
        public string internalName;

        /// <summary>
        /// If true, look up the localized field associated with the actor's name.
        /// </summary>
        [Tooltip("Look up the localized field associated with the actor's name.")]
        public bool useLocalizedNameInDatabase = false;

        public void OnEnable()
        {
            if (string.IsNullOrEmpty(overrideName)) return;
            CharacterInfo.RegisterActorTransform(overrideName, transform);
        }

        public void OnDisable()
        {
            if (string.IsNullOrEmpty(overrideName)) return;
            CharacterInfo.UnregisterActorTransform(overrideName, transform);
        }

        /// <summary>
        /// Gets the name, which is possibly the override name or its localized version.
        /// </summary>
        /// <returns>The name.</returns>
        public string GetName()
        {
            var actorName = string.IsNullOrEmpty(overrideName) ? name : overrideName;
            if (useLocalizedNameInDatabase)
            {
                var result = CharacterInfo.GetLocalizedDisplayNameInDatabase(DialogueLua.GetActorField(actorName, "Name").AsString);
                return (!string.IsNullOrEmpty(result)) ? result : actorName;
            }
            else
            {
                return actorName;
            }
        }

        /// <summary>
        /// Gets the name of the override, including parsing if it contains a [lua]
        /// or [var] tag.
        /// </summary>
        /// <returns>The override name, or <c>null</c> if not set.</returns>
        public string GetOverrideName()
        {
            if (overrideName.Contains("[lua") || overrideName.Contains("[var"))
            {
                return FormattedText.Parse(overrideName, DialogueManager.MasterDatabase.emphasisSettings).text;
            }
            else
            {
                return overrideName;
            }
        }

        public string GetInternalName()
        {
            return string.IsNullOrEmpty(internalName) ? GetOverrideName() : internalName;
        }

        /// <summary>
        /// Searches a GameObject or its parent for an OverrideActorName component.
        /// </summary>
        /// <param name="t">The GameObject to search.</param>
        /// <returns>The OverrideActorName component, or null if neither the GameObject nor its parent has one.</returns>
        public static OverrideActorName GetOverrideActorName(Transform t)
        {
            if (t == null) return null;
            var overrideActorName = t.GetComponentInChildren<OverrideActorName>();
            if (overrideActorName == null && t.parent != null) overrideActorName = t.parent.GetComponent<OverrideActorName>();
            return overrideActorName;
        }

        /// <summary>
        /// Gets the name of the actor, either from the GameObject or its OverrideActorComponent
        /// if present.
        /// </summary>
        /// <returns>The actor name.</returns>
        /// <param name="t">The actor's transform.</param>
        public static string GetActorName(Transform t)
        {
            if (t == null) return string.Empty;
            var overrideActorName = GetOverrideActorName(t);
            return (overrideActorName == null) ? CharacterInfo.GetLocalizedDisplayNameInDatabase(t.name) : overrideActorName.GetName();
        }

        /// <summary>
        /// Gets the internal database name of the actor, from the OverrideActorComponent's internalName
        /// if set, otherwise the overrideName, or the GameObject name if the GameObject doesn't have an
        /// OverrideActorName component.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetInternalName(Transform t)
        {
            if (t == null) return string.Empty;
            var overrideActorName = GetOverrideActorName(t);
            if (overrideActorName != null)
            {
                if (!string.IsNullOrEmpty(overrideActorName.internalName)) return overrideActorName.internalName;
                if (!string.IsNullOrEmpty(overrideActorName.overrideName)) return overrideActorName.overrideName;
            }
            return t.name;
        }

    }

}
