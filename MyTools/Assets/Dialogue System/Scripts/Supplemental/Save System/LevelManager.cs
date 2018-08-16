using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem {

    /// <summary>
    /// This component combines Application.LoadLevel[Async] with the saved-game data
    /// features of PersistentDataManager. To use it, add it to your Dialogue Manager
    /// object and pass the saved-game data to LevelManager.LoadGame().
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/level_manager.html")]
#endif
    [AddComponentMenu("Dialogue System/Save System/Level Manager")]
	public class LevelManager : MonoBehaviour {
		
		/// <summary>
		/// The default starting level to use if none is recorded in the saved-game data.
		/// </summary>
		public string defaultStartingLevel;
		
		/// <summary>
		/// Indicates whether a level is currently loading. Only useful in Unity Pro, which
		/// uses Application.LoadLevelAsync().
		/// </summary>
		/// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
		public bool IsLoading { get; private set; }
		
		protected virtual void Awake() {
			IsLoading = false;
		}

        protected virtual void OnEnable()
        {
            PersistentDataManager.RegisterPersistentData(gameObject);
        }

        protected virtual void OnDisable()
        {
            PersistentDataManager.UnregisterPersistentData(gameObject);
        }
		
		/// <summary>
		/// Loads the game recorded in the provided saveData.
		/// </summary>
		/// <param name="saveData">Save data.</param>
		public void LoadGame(string saveData) {
			StartCoroutine(LoadLevelFromSaveData(saveData));
		}
		
		/// <summary>
		/// Restarts the game at the default starting level and resets the
		/// Dialogue System to its initial database state.
		/// </summary>
		public void RestartGame() {
			StartCoroutine(LoadLevelFromSaveData(null));
		}
		
		private IEnumerator LoadLevelFromSaveData(string saveData) {
            if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager: Starting LoadLevelFromSaveData coroutine");
            string levelName = defaultStartingLevel;
			if (string.IsNullOrEmpty(saveData)) {
                // If no saveData, reset the database.
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager: Save data is empty, so just resetting database");
                DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
			} else {
                // Put saveData in Lua so we can get Variable["SavedLevelName"]:
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager: Applying save data to get value of 'SavedLevelName' variable");
                Lua.Run(saveData, DialogueDebug.LogInfo);
				levelName = DialogueLua.GetVariable("SavedLevelName").AsString;
                if (string.IsNullOrEmpty(levelName) || string.Equals(levelName, "nil")) {
                    levelName = defaultStartingLevel;
                    if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager: 'SavedLevelName' isn't defined. Using default level " + levelName);
                }
                else {
                    if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager: SavedLevelName = " + levelName);
                }
			}
			
			// Load the level:
			PersistentDataManager.LevelWillBeUnloaded();

			if (CanLoadAsync()) {
				AsyncOperation async = Tools.LoadLevelAsync(levelName); //---Was: Application.LoadLevelAsync(levelName);
				IsLoading = true;
				while (!async.isDone) {
					yield return null;
				}
				IsLoading = false;
			} else {
				Tools.LoadLevel(levelName); //---Was: Application.LoadLevel(levelName);
			}

            // Wait two frames for objects in the level to finish their Start() methods:
            if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager finished loading level " + levelName + ". Waiting 2 frames for scene objects to start.");
			yield return null;
			yield return null;
			
			// Then apply saveData to the objects:
			if (!string.IsNullOrEmpty(saveData)) {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: LevelManager waited 2 frames. Appling save data: " + saveData);
                PersistentDataManager.ApplySaveData(saveData);
			}

            // Update quest tracker HUD:
            DialogueManager.SendUpdateTracker();
		}
		
		/// <summary>
		/// Loads a level. Use to change levels while keeping data synced. This method
		/// also calls PersistentDataManager.Record() before changing levels and
		/// PersistentDataManager.Apply() after changing levels. After loading the level,
		/// it waits two frames to allow GameObjects to finish their initialization first.
		/// </summary>
		/// <param name="levelName">Level name.</param>
		public void LoadLevel(string levelName) {
			StartCoroutine(LoadLevelCoroutine(levelName, -1));
		}

        /// <summary>
        /// Loads a level. Use to change levels while keeping data synced. This method
        /// also calls PersistentDataManager.Record() before changing levels and
        /// PersistentDataManager.Apply() after changing levels. After loading the level,
        /// it waits two frames to allow GameObjects to finish their initialization first.
        /// </summary>
        /// <param name="levelIndex">Scene index in build settings.</param>
        public void LoadLevel(int levelIndex)
        {
            StartCoroutine(LoadLevelCoroutine(null, levelIndex));
        }

        private IEnumerator LoadLevelCoroutine(string levelName, int levelIndex) {
			PersistentDataManager.Record();

			// Load the level:
			PersistentDataManager.LevelWillBeUnloaded();
			if (CanLoadAsync()) {
                AsyncOperation async = !string.IsNullOrEmpty(levelName) ? Tools.LoadLevelAsync(levelName) : Tools.LoadLevelAsync(levelIndex);
				IsLoading = true;
				while (!async.isDone) {
					yield return null;
				}
				IsLoading = false;
			} else {
                if (!string.IsNullOrEmpty(levelName)) Tools.LoadLevel(levelName); else Tools.LoadLevel(levelIndex);
			}
			
			// Wait two frames for objects in the level to finish their Start() methods:
			yield return null;
			yield return null;

			// Apply position data, but don't apply player's position:
			var player = GameObject.FindGameObjectWithTag("Player");
			var persistentPos = (player != null) ? player.GetComponent<PersistentPositionData>() : null;
			var originalValue = false;
			if (persistentPos != null) {
				originalValue = persistentPos.restoreCurrentLevelPosition;
				persistentPos.restoreCurrentLevelPosition = false;
			}
			
			PersistentDataManager.Apply();
			if (persistentPos != null) {
				persistentPos.restoreCurrentLevelPosition = originalValue;
			}

            // Update quest tracker HUD:
            DialogueManager.SendUpdateTracker();
        }

        private bool CanLoadAsync() {
			#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			return Application.HasProLicense();
			#else
			return true;
			#endif
		}
		
		/// <summary>
		/// Records the current level in Lua.
		/// </summary>
		public virtual void OnRecordPersistentData() {
			DialogueLua.SetVariable("SavedLevelName", Tools.loadedLevelName);
		}
		
	}
	
}