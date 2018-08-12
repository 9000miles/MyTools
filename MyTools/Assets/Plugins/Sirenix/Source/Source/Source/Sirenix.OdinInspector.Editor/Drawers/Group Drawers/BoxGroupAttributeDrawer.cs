#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="BoxGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="BoxGroupAttribute"/>
    /// </summary>
    /// <seealso cref="BoxGroupAttribute"/>
    [OdinDrawer]
    public class BoxGroupAttributeDrawer : OdinGroupDrawer<BoxGroupAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, BoxGroupAttribute attribute, GUIContent label)
        {
            var labelGetter = property.Context.Get<StringMemberHelper>(this, "LabelContext", (StringMemberHelper)null);
            if (labelGetter.Value == null)
            {
                labelGetter.Value = new StringMemberHelper(property.ParentType, attribute.GroupName);
            }

            // -------------------------------------------
            // The following outcommented code makes support for drawing a referenced $property in the header, instead of a label.
            // However it felt a little weird. So I'll leave it outcommented for now.
            // -------------------------------------------
            //if (labelGetter.Value.IsDynamicString == false && attribute.Label != null && attribute.Label[0] == '$')
            //{
            //    PropertyContext<bool> isReferencingProperty;
            //    if (property.Context.Get(this, "test", out isReferencingProperty))
            //    {
            //        string path = attribute.Label;
            //        path = path.Substring(1);
            //        var p = property.Children.Get(path);
            //        isReferencingProperty.Value = p != null;
            //    }
            //
            //    if (isReferencingProperty.Value)
            //    {
            //        var toggleProperty = property.Children.Get(attribute.Label.Substring(1));
            //        SirenixEditorGUI.BeginBox();
            //        SirenixEditorGUI.BeginBoxHeader(false);
            //        InspectorUtilities.DrawProperty(toggleProperty);
            //        SirenixEditorGUI.EndBoxHeader();
            //
            //        var val = toggleProperty.ValueEntry.WeakSmartValue;
            //        bool disableGUI = false;
            //
            //        if (val != null && val.Equals(false))
            //        {
            //            disableGUI = true;
            //        }
            //
            //        if (disableGUI)
            //        {
            //            GUIHelper.PushGUIEnabled(false);
            //        }
            //
            //        for (int i = 0; i < property.Children.Count; i++)
            //        {
            //            var child = property.Children[i];
            //            if (child != toggleProperty)
            //            {
            //                InspectorUtilities.DrawProperty(child);
            //            }
            //        }
            //
            //        if (disableGUI)
            //        {
            //            GUIHelper.PopGUIEnabled();
            //        }
            //
            //        SirenixEditorGUI.EndBox();
            //        return;
            //    }
            //}
            // -------------------------------------------

            SirenixEditorGUI.BeginBox(attribute.ShowLabel ? labelGetter.Value.GetString(property) : null, attribute.CenterLabel);

            for (int i = 0; i < property.Children.Count; i++)
            {
                InspectorUtilities.DrawProperty(property.Children[i]);
            }

            SirenixEditorGUI.EndBox();
        }
    }
}
#endif