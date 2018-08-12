#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FilePathAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    [OdinDrawer]
    public sealed class FilePathAttributeDrawer : OdinAttributeDrawer<FilePathAttribute, string>, IDefinesGenericMenuItems
    {
        private class FilePathContext
        {
            public string ErrorMessage;
            public StringMemberHelper Parent;
            public StringMemberHelper Extensions;
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<string> entry, FilePathAttribute attribute, GUIContent label)
        {
            // Context
            InspectorProperty parentProperty = entry.Property.FindParent(PropertyValueCategory.Member, true);
            PropertyContext<FilePathContext> context;
            if (entry.Context.Get(this, "FilePathContext", out context))
            {
                context.Value = new FilePathContext();

                string p = attribute.ParentFolder != null ? attribute.ParentFolder : attribute.AbsolutePath ? null : Directory.GetParent(Application.dataPath).FullName;

                context.Value.Parent = new StringMemberHelper(parentProperty.ParentType, p, ref context.Value.ErrorMessage);
                context.Value.Extensions = new StringMemberHelper(parentProperty.ParentType, attribute.Extensions, ref context.Value.ErrorMessage);
            }

            // Initialization errors
            if (context.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
                this.CallNextDrawer(entry, label);
                return;
            }

            // Parent
            string extensions = context.Value.Extensions.GetString(parentProperty);
            string parent = context.Value.Parent.GetString(parentProperty);
            if (!parent.IsNullOrWhitespace())
            {
                parent = Path.IsPathRooted(parent) ? parent : Path.GetFullPath(parent);
                parent = attribute.UseBackslashes ? parent.Replace('/', '\\') : parent.Replace('\\', '/');
            }

            // Value errors
            if (attribute.RequireValidPath && (entry.SmartValue.IsNullOrWhitespace() || !File.Exists(this.GetFullPath(entry.SmartValue, parent, attribute.AbsolutePath))))
            {
                SirenixEditorGUI.ErrorMessageBox("File does not exist.");
            }
            else if (attribute.AbsolutePath && !entry.SmartValue.IsNullOrWhitespace() && !Path.IsPathRooted(entry.SmartValue))
            {
                SirenixEditorGUI.ErrorMessageBox("Path is not absolute.");
            }
            else if (!entry.SmartValue.IsNullOrWhitespace() && !this.ValidateExtension(entry.SmartValue, extensions))
            {
                PropertyContext<bool> isFolded = entry.Property.Context.Get<bool>(this, "ExtensionErrorMessage", true);
                isFolded.Value = SirenixEditorGUI.DetailedMessageBox("Extension does not match specified.", "Valid extensions are: " + this.NicifyExtensions(extensions), MessageType.Error, isFolded.Value);
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

                    if (!((a & FileAttributes.Directory) == FileAttributes.Directory) &&
                        rect.Contains(Event.current.mousePosition) &&
                        this.ValidateExtension(DragAndDrop.paths[0], extensions))
                    {
                        valid = true;
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                }

                if (!attribute.ReadOnly && valid && Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
                {
                    entry.SmartValue = Process(DragAndDrop.paths[0], parent, attribute.AbsolutePath, attribute.UseBackslashes);
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                }
            }

            // Label
            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            // Field
            EditorGUI.BeginChangeCheck();
            string buffer;

            if (attribute.ReadOnly)
            {
                buffer = entry.SmartValue;
                EditorGUI.TextField(rect.AlignLeft(rect.width - 18), buffer);
            }
            else
            {
                buffer = EditorGUI.TextField(rect.AlignLeft(rect.width - 18), entry.SmartValue);
            }

            if (EditorGUI.EndChangeCheck() && !attribute.ReadOnly)
            {
                entry.SmartValue = buffer;
            }

            // Select
            if (SirenixEditorGUI.IconButton(rect.AlignRight(18f).SetHeight(18f).SubY(1).AddX(1), EditorIcons.Folder))
            {
                string dir = entry.SmartValue.IsNullOrWhitespace() ? context.Value.Parent.GetString(parentProperty) : entry.SmartValue;
                if (attribute.ReadOnly)
                {
                    System.Diagnostics.Process.Start(Path.GetDirectoryName(dir));
                }
                else
                {
                    buffer = EditorUtility.OpenFilePanel("Select file", Path.GetDirectoryName(dir), this.ProcessExtensions(extensions));
                    if (!string.IsNullOrEmpty(buffer))
                    {
                        entry.SmartValue = Process(buffer, parent, attribute.AbsolutePath, attribute.UseBackslashes);
                    }
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
            if (parentPath.IsNullOrWhitespace())
            {
                return path;
            }
            if (path.IsNullOrWhitespace())
            {
                return null;
            }

            try
            {
                Uri pathUri = new Uri(path, UriKind.Absolute);
                Uri parentUri = new Uri(parentPath + "\\Dummy", UriKind.Absolute);

                return parentUri.MakeRelativeUri(pathUri).ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// EditorUtility.OpenFilePanel's extension parameter is a bit sensitive to formatting.
        /// This methods processes whatever the user inputs as extensions, to make it easier for the user.
        /// </summary>
        private string ProcessExtensions(string extensions)
        {
            if (extensions.IsNullOrWhitespace())
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            var e = extensions.Split(',', ';').Select(i => i.Trim(' ', '.', '*')).Where(i => !i.IsNullOrWhitespace()).GetEnumerator();

            while (e.MoveNext())
            {
                if (builder.Length > 0)
                {
                    builder.Append(";*.");
                }
                builder.Append(e.Current);
            }

            return builder.ToString();
        }

        private string NicifyExtensions(string extensions)
        {
            if (extensions.IsNullOrWhitespace())
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            var e = extensions.Split(',', ';').Select(i => i.Trim(' ', '.', '*')).Where(i => !i.IsNullOrWhitespace()).GetEnumerator();

            while (e.MoveNext())
            {
                builder.Append("\n.");
                builder.Append(e.Current);
            }

            return builder.Length > 0 ? builder.ToString() : null;
        }

        private bool ValidateExtension(string path, string extensions)
        {
            if (extensions.IsNullOrWhitespace())
            {
                return true;
            }
            if (path.IsNullOrWhitespace())
            {
                return false;
            }

            string e = Path.GetExtension(path).ToLower();
            if (e.IsNullOrWhitespace())
            {
                return false;
            }

            return extensions.Split(',', ';').Select(i => "." + i.Trim(' ', '.', '*').ToLower()).Any(i => i == e);
        }

        private string GetFullPath(string path, string parent, bool absolute)
        {
            if (path.IsNullOrWhitespace())
            {
                return null;
            }

            return absolute ? path : Path.GetFullPath(parent.IsNullOrWhitespace() ? path.TrimStart('/', '\\') : Path.Combine(parent, path.TrimStart('/', '\\')));
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            var attribute = property.Info.GetAttribute<FilePathAttribute>();
            var parentProperty = property.FindParent(PropertyValueCategory.Member, true);
            IPropertyValueEntry<string> entry = (IPropertyValueEntry<string>)property.ValueEntry;
            string parent = entry.Context.Get<FilePathContext>(this, "FilePathContext", (FilePathContext)null).Value.Parent.GetString(parentProperty);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            // Clear
            if (!attribute.ReadOnly)
            {
                genericMenu.AddItem(new GUIContent("Clear path"), false, () =>
                {
                    entry.Property.Tree.DelayActionUntilRepaint(() =>
                    {
                        entry.SmartValue = null;
                    });
                });
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Clear path"));
            }

            string fullpath = GetFullPath(entry.SmartValue, parent, attribute.AbsolutePath);

            // Show in explorer
            if (!fullpath.IsNullOrWhitespace() && Directory.Exists(Path.GetDirectoryName(fullpath)))
            {
                genericMenu.AddItem(new GUIContent("Show in explorer"), false, () => System.Diagnostics.Process.Start(Path.GetDirectoryName(fullpath)));
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Show in explorer"));
            }
        }
    }
}
#endif