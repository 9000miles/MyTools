#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FolderPathAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    [OdinDrawer]
    public sealed class FolderPathAttributeDrawer : OdinAttributeDrawer<FolderPathAttribute, string>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<string> entry, FolderPathAttribute attribute, GUIContent label)
        {
            var parentProperty = entry.Property.FindParent(PropertyValueCategory.Member, true);

            // Parent path
            PropertyContext<StringMemberHelper> context;
            if (entry.Property.Context.Get<StringMemberHelper>(this, "Parent", out context))
            {
                string p = attribute.ParentFolder != null ? attribute.ParentFolder : attribute.AbsolutePath ? null : Directory.GetParent(Application.dataPath).FullName;
                context.Value = new StringMemberHelper(parentProperty.ParentType, p);
            }

            // Error message
            if (context.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
                this.CallNextDrawer(entry, label);
                return;
            }

            string parent = context.Value.GetString(parentProperty);
            if (!parent.IsNullOrWhitespace())
            {
                parent = Path.IsPathRooted(parent) ? parent : Path.GetFullPath(parent);
                parent = attribute.UseBackslashes ? parent.Replace('/', '\\') : parent.Replace('\\', '/');
            }

            // Value errors
            if (attribute.RequireValidPath && (entry.SmartValue.IsNullOrWhitespace() || !Directory.Exists(this.GetFullPath(entry.SmartValue, parent, attribute.AbsolutePath))))
            {
                SirenixEditorGUI.ErrorMessageBox("Path does not exist.");
            }
            else if (attribute.AbsolutePath && !entry.SmartValue.IsNullOrWhitespace() && !Path.IsPathRooted(entry.SmartValue))
            {
                SirenixEditorGUI.ErrorMessageBox("Path is not absolute.");
            }

            Rect rect = EditorGUILayout.GetControlRect();

            // Highlight
            GUIHelper.PushGUIEnabled(true);
            if (label != null && rect.AlignLeft(EditorGUIUtility.labelWidth).Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.clickCount >= 2)
            {
                string path = this.MakeRelative(this.GetFullPath(entry.SmartValue, parent, attribute.AbsolutePath), Directory.GetParent(Application.dataPath).FullName);

                if (path != null)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

                    if (obj != null)
                    {
                        EditorGUIUtility.PingObject(obj);
                    }
                }

                Event.current.Use();
            }
            GUIHelper.PopGUIEnabled();

            // Drag and drop
            if (DragAndDrop.objectReferences.Length == 1)
            {
                bool valid = false;
                if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                {
                    var a = File.GetAttributes(DragAndDrop.paths[0]);

                    if ((a & FileAttributes.Directory) == FileAttributes.Directory && rect.Contains(Event.current.mousePosition))
                    {
                        valid = true;
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                }

                if (valid && Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
                {
                    entry.SmartValue = Process(DragAndDrop.paths[0], parent, attribute.AbsolutePath, attribute.UseBackslashes);
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                }
            }

            // Field
            EditorGUI.BeginChangeCheck();
            string buffer = SirenixEditorFields.TextField(rect.AlignLeft(rect.width - 18), label, entry.SmartValue);
            if (EditorGUI.EndChangeCheck())
            {
                // Don't use process the path here, to allow users to input whatever they desire.
                entry.SmartValue = attribute.UseBackslashes ? buffer.Replace('/', '\\') : buffer.Replace('\\', '/');
            }

            // Select
            if (SirenixEditorGUI.IconButton(rect.AlignRight(18f).SetHeight(18f).SubY(1).AddX(1), EditorIcons.Folder))
            {
                string directory = (entry.SmartValue.IsNullOrWhitespace() ? parent : entry.SmartValue) ?? "";

                buffer = EditorUtility.OpenFolderPanel("Select output directory", directory.TrimStart('/', '\\'), "");

                if (!buffer.IsNullOrWhitespace())
                {
                    entry.SmartValue = Process(buffer, parent, attribute.AbsolutePath, attribute.UseBackslashes);
                }
            }
        }

        private string Process(string path, string parentPath, bool absolute, bool useBackslashes)
        {
            if (path.IsNullOrWhitespace())
            {
                return null;
            }

            // Get absolute path.
            path = Path.GetFullPath(path);

            // Make relative.
            if (!absolute && parentPath != null)
            {
                path = this.MakeRelative(path, parentPath);
            }

            return useBackslashes ? path.Replace('/', '\\') : path.Replace('\\', '/');
        }

        private string MakeRelative(string path, string parentPath)
        {
            if (path.IsNullOrWhitespace() || parentPath.IsNullOrWhitespace())
            {
                return null;
            }

            Uri pathUri = new Uri(path, UriKind.Absolute);
            Uri parentUri = new Uri(parentPath + "\\Dummy", UriKind.Absolute);
            return parentUri.MakeRelativeUri(pathUri).ToString();
        }

        private string GetFullPath(string path, string parent, bool absolute)
        {
            if (path.IsNullOrWhitespace())
            {
                return null;
            }

            return absolute ? path : Path.GetFullPath(parent.IsNullOrWhitespace() ? path.TrimStart('/', '\\') : Path.Combine(parent, path.TrimStart('/', '\\')));
        }

        /// <summary>
        /// Adds customs generic menu options.
        /// </summary>
        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            var attribute = property.Info.GetAttribute<FolderPathAttribute>();
            var parentProperty = property.FindParent(PropertyValueCategory.Member, true);
            IPropertyValueEntry<string> entry = (IPropertyValueEntry<string>)property.ValueEntry;
            string parent = entry.Context.Get<StringMemberHelper>(this, "Parent", (StringMemberHelper)null).Value.GetString(parentProperty);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            // Clear.
            genericMenu.AddItem(new GUIContent("Clear path"), false, () =>
            {
                entry.Property.Tree.DelayActionUntilRepaint(() =>
                {
                    entry.SmartValue = null;
                });
            });

            string fullpath = this.GetFullPath(entry.SmartValue, parent, attribute.AbsolutePath);
            bool exists = !fullpath.IsNullOrWhitespace() && Directory.Exists(fullpath);

            // Show in explorer
            if (exists)
            {
                genericMenu.AddItem(new GUIContent("Show in explorer"), false, () => Application.OpenURL(fullpath));
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Show in explorer"));
            }

            // Create path
            if (entry.SmartValue.IsNullOrWhitespace() || exists)
            {
                genericMenu.AddDisabledItem(new GUIContent("Create directory"));
            }
            else
            {
                genericMenu.AddItem(new GUIContent("Create path"), false, () =>
                {
                    Directory.CreateDirectory(fullpath);
                    AssetDatabase.Refresh();
                });
            }
        }
    }
}
#endif