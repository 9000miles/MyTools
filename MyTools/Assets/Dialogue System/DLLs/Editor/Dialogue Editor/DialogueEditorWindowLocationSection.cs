﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.DialogueEditor
{

    /// <summary>
    /// This part of the Dialogue Editor window handles the Locations tab. Locations are
    /// just treated as basic assets, so it uses the generic asset methods.
    /// </summary>
    public partial class DialogueEditorWindow
    {

        [SerializeField]
        private AssetFoldouts locationFoldouts = new AssetFoldouts();

        [SerializeField]
        private string locationFilter = string.Empty;

        private void ResetLocationSection()
        {
            locationFoldouts = new AssetFoldouts();
            locationAssetList = null;
        }

        private void DrawLocationSection()
        {
            if (database.syncInfo.syncLocations)
            {
                DrawAssetSection<Location>("Location", database.locations, locationFoldouts, DrawLocationMenu, DrawLocationSyncDatabase, ref locationFilter);
            }
            else {
                DrawAssetSection<Location>("Location", database.locations, locationFoldouts, DrawLocationMenu, ref locationFilter);
            }
        }

        private void DrawLocationMenu()
        {
            if (GUILayout.Button("Menu", "MiniPullDown", GUILayout.Width(56)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("New Location"), false, AddNewLocation);
                menu.AddItem(new GUIContent("Sort/By Name"), false, SortLocationsByName);
                menu.AddItem(new GUIContent("Sort/By ID"), false, SortLocationsByID);
                menu.AddItem(new GUIContent("Sync From DB"), database.syncInfo.syncLocations, ToggleSyncLocationsFromDB);
                menu.ShowAsContext();
            }
        }

        private void AddNewLocation()
        {
            AddNewAsset<Location>(database.locations);
            SetDatabaseDirty("Add New Location");
        }

        private void SortLocationsByName()
        {
            database.locations.Sort((x, y) => x.Name.CompareTo(y.Name));
            SetDatabaseDirty("Sort Locations by Name");
        }

        private void SortLocationsByID()
        {
            database.locations.Sort((x, y) => x.id.CompareTo(y.id));
            SetDatabaseDirty("Sort Locations by ID");
        }

        private void ToggleSyncLocationsFromDB()
        {
            database.syncInfo.syncLocations = !database.syncInfo.syncLocations;
            SetDatabaseDirty("Toggle Sync Locations");
        }

        private void DrawLocationSyncDatabase()
        {
            EditorGUILayout.BeginHorizontal();
            DialogueDatabase newDatabase = EditorGUILayout.ObjectField(new GUIContent("Sync From", "Database to sync locations from."),
                                                                       database.syncInfo.syncLocationsDatabase, typeof(DialogueDatabase), false) as DialogueDatabase;
            if (newDatabase != database.syncInfo.syncLocationsDatabase)
            {
                database.syncInfo.syncLocationsDatabase = newDatabase;
                database.SyncLocations();
                SetDatabaseDirty("Change Location Sync Database");
            }
            if (GUILayout.Button(new GUIContent("Sync Now", "Syncs from the database."), EditorStyles.miniButton, GUILayout.Width(72)))
            {
                database.SyncLocations();
                SetDatabaseDirty("Manual Sync Locations");
            }
            EditorGUILayout.EndHorizontal();
        }

    }

}