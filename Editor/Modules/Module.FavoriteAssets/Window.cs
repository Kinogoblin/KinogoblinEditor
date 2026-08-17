using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class Window : EditorWindow
    {
        private const string QCWINDOWNAME = "Favorite Assets";
        private const string VERSION = "1.19.0";

        private const int StateHistory = 0;
        private const int StateFavorites = 1;
        private const int StateSceneFavorites = 2;
        private const int StateSceneData = 3;

        private int m_state;

        //Mirrors m_state so non-window code (the favorite hotkey) can tell which tab is active
        //without needing a window instance reference.
        private static int s_activeState = StateHistory;
        internal static bool IsSceneFavoritesTabActive { get { return s_activeState == StateSceneFavorites; } }

        private ProjectData m_projectData;

        private Vector2 m_mainScrollPos;

        private Texture m_iconDelete;
        private Texture m_iconSettings;
        private Texture m_iconUsers;

        GUIContent content_favorites = new GUIContent();
        GUIContent content_history = new GUIContent();
        GUIContent content_sceneFavorites = new GUIContent();
        GUIContent content_sceneData = new GUIContent();

        private Dictionary<string, bool> m_favoriteFolderFoldoutDict = new Dictionary<string, bool>();
        private string m_favoriteSearchString = "";

        private const int GroupModeByType = 0;
        private const int GroupModeByList = 1;
        private int m_favoritesGroupMode;
        private string m_newListName = "";

        //Checking if we have added or pasted new objects through commandname
        private bool m_newObjectAdded;

        private const string StyleNameSearchTextField = "ToolbarSearchTextFieldPopup";
        private const string StyleNameSeachTextField = "ToolbarSeachTextFieldPopup";
        private string _styleTextFieldName
        {
            get
            {
                if (GUI.skin.FindStyle(StyleNameSearchTextField) == null)
                {
                    return StyleNameSeachTextField;
                }

                return StyleNameSearchTextField;
            }
        }
        private const string StyleNameSearchCancelButton = "ToolbarSearchCancelButton";
        private const string StyleNameSeachCancelButton = "ToolbarSeachCancelButton";
        private string _styleSearchCancelButton
        {
            get 
            {
                if (GUI.skin.FindStyle(StyleNameSearchCancelButton) == null)
                {
                    return StyleNameSeachCancelButton;
                }
                return StyleNameSearchCancelButton;
            }
        }

        private void Awake()
        {
            if (Preferences.DefaultToEnvironmentUser)
                Preferences.ClearUserOverride();

            if (EditorPrefs.HasKey(Preferences.QSPrefsMainWindowState))
                m_state = EditorPrefs.GetInt(Preferences.QSPrefsMainWindowState);

            if (EditorPrefs.HasKey(Preferences.QSPrefsHistorySortState))
                ProfileData.Instance.HistorySortState = EditorPrefs.GetInt(Preferences.QSPrefsHistorySortState);

            if (EditorPrefs.HasKey(Preferences.QSPrefsFavoritesGroupMode))
                m_favoritesGroupMode = EditorPrefs.GetInt(Preferences.QSPrefsFavoritesGroupMode);
        }

        internal static void Init(bool bShow)
        {
            //With docking params set. Current commented out
            Window m_window = EditorWindow.GetWindow<Window>(false, QCWINDOWNAME, bShow);
            m_window.titleContent.image = ConfigData.Instance.WindowPaneIcon;
            m_window.titleContent.text = QCWINDOWNAME;

            m_window.m_iconDelete = ConfigData.Instance.DeleteIcon;
            m_window.m_iconSettings = ConfigData.Instance.SettingsIcon;
            m_window.m_iconUsers = ConfigData.Instance.UsersIcon;
        }

        public void OnEnable()
        {
            //Set title/icons on this instance directly - do NOT call GetWindow<Window>() here,
            //since OnEnable/Awake fire whenever Unity reconstructs this window (e.g. layout
            //rebuilds when toggling Maximize on Play / fullscreen Game View), and a re-entrant
            //GetWindow call during that reconstruction can spawn a duplicate, undocked window.
            titleContent.image = ConfigData.Instance.WindowPaneIcon;
            titleContent.text = QCWINDOWNAME;

            m_iconDelete = ConfigData.Instance.DeleteIcon;
            m_iconSettings = ConfigData.Instance.SettingsIcon;
            m_iconUsers = ConfigData.Instance.UsersIcon;

            //New Projectdata
            m_projectData = new ProjectData();
            m_projectData.Init();

            verifyFavorites();

            SceneView.RepaintAll();

            //Subscribe to event to check for paste/Duplicate events
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        public void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            checkForEditorCommands(instanceID);

            if (!Selection.instanceIDs.Contains<int>(instanceID))
                return;
        }

        //Check if scene recieved paste/duplicate commands
        private void checkForEditorCommands(int instanceID)
        {
            if (!Selection.instanceIDs.Contains<int>(instanceID))
                return;

            Event e = Event.current;
            if (e.type == EventType.ValidateCommand || e.type == EventType.ExecuteCommand)
            {
                //Mark that we have added new object through commands, will then deal with it in "SelectionChanged", reason is that "Selection.gameobjects" returned original objects, not the newly created
                if (e.commandName == "Duplicate" || e.commandName == "Paste")
                    m_newObjectAdded = true;
            }
        }

        void OnSelectionChange()
        {
            if (m_newObjectAdded)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    ProfileData.Instance.AddToHistory(go);
                }

                m_newObjectAdded = false;
            }
        }

        public void OnDestroy()
        {
            SceneView.RepaintAll();
        }

        //Delegate for scene change (i.e object added)
        private void OnHierarchyChange()
        {
            if (!Application.isPlaying)
            {
                m_projectData.UpdateSceneInfo();
                this.Repaint();
            }
        }

        //Delegate for project change (i.e asset added/removed)
        private void OnProjectChange()
        {
            verifyFavorites();
        }

        //Verifies that data is up to date (Check for delete etc)
        private void verifyFavorites()
        {
            //AssetDatabase.Refresh();
            if (ProfileData.Instance != null)
            {
                ProfileData.Instance.CleanUserData();
                m_projectData.VerifyData();
            }
        }

        //Force repaint
        public void OnInspectorUpdate()
        {
            // This will only get called 10 times per second.
            Repaint();
        }

        void OnGUI()
        {
            s_activeState = m_state;

            WindowStyler.DrawGlobalHeader(this, ConfigData.Instance.WindowPaneIcon, WindowStyler.clr_lBlue, QCWINDOWNAME,VERSION);
            drawDefault();

            m_mainScrollPos = EditorGUILayout.BeginScrollView(m_mainScrollPos);

            switch (m_state)
            {
                case StateHistory:
                    drawHistory();
                    break;
                case StateFavorites:
                    drawUserFavorites();
                    break;
                case StateSceneFavorites:
                    drawSceneFavorites();
                    break;
                case StateSceneData:
                    drawSceneview();
                    break;
                default:
                    Debug.LogError("Could not find relevant state");
                    break;
            }
            EditorGUILayout.EndScrollView();

            if (Preferences.ShowShortcutHint)
            {
                /*GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(VERSION, EditorStyles.miniLabel);
                GUILayout.EndHorizontal();*/
                EditorGUILayout.LabelField("Press CTRL+SHIFT+T to add an asset or folder to favorites", EditorStyles.toolbar, GUILayout.MaxHeight(10));
            }
        }

        private void drawDefault()
        {
            content_history.image = ConfigData.Instance.HistoryIcon;
            content_history.text = Preferences.ShowMenuText ? "History" : "";
            content_history.tooltip = "Recently used assets";

            content_favorites.image = ConfigData.Instance.FavoriteIcon;
            content_favorites.text = Preferences.ShowMenuText ? "Favorites" : "";
            content_favorites.tooltip = "Personal favorites";

            content_sceneFavorites.image = ConfigData.Instance.SelectedIcon;
            content_sceneFavorites.text = Preferences.ShowMenuText ? "Scene Favorites" : "";
            content_sceneFavorites.tooltip = "Favorite objects per open scene - click one to select it in the scene";

            content_sceneData.image = ConfigData.Instance.SceneIcon;
            content_sceneData.text = Preferences.ShowMenuText ? "Scene data" : "";
            content_sceneData.tooltip = "Scene asset usage";

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            m_state = GUILayout.Toolbar(m_state, new GUIContent[] { content_history, content_favorites, content_sceneFavorites, content_sceneData }, GUILayout.Height(26), GUILayout.MaxWidth(Preferences.ShowMenuText ? 390 : 170));
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetInt(Preferences.QSPrefsMainWindowState, m_state);

            if (GUILayout.Button(m_iconSettings, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                PreferencesWindow.Init();
            if (GUILayout.Button(m_iconUsers, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                ProfileWindow.Init();

            EditorGUILayout.BeginVertical();
            GUILayout.Space(14);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void drawSceneview()
        {
            bool showThumbNails = Preferences.ShowThumbnails;
            int showCount = Preferences.SceneCountLimit;

            int showCounter = 0;
            if (m_projectData.HasValidSceneData())
                foreach (string assetIDs in m_projectData.GetSceneAssetIdentifiers())
                {
                    UIStyler.DrawAssetFromID(assetIDs, showThumbNails);
                    showCounter++;
                    //Break loop
                    if (showCounter >= showCount)
                        break;
                }
        }

        private void drawSceneFavorites()
        {
            int openSceneCount = EditorSceneManager.sceneCount;
            bool anySceneDrawn = false;

            for (int i = 0; i < openSceneCount; i++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                    continue;

                anySceneDrawn = true;
                drawSceneFavoritesForScene(scene);
                EditorGUILayout.Space();
            }

            if (!anySceneDrawn)
                EditorGUILayout.LabelField("No open scenes.");
        }

        private void drawSceneFavoritesForScene(Scene scene)
        {
            string sceneGUID = ProfileData.Instance.GetSceneGUID(scene);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            UnityEngine.Object sceneAsset = string.IsNullOrEmpty(scene.path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
            EditorGUILayout.ObjectField(scene.name + (scene.isDirty ? "*" : ""), sceneAsset, typeof(SceneAsset), false);

            if (string.IsNullOrEmpty(sceneGUID))
            {
                EditorGUILayout.HelpBox("Save this scene to be able to favorite objects in it.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            SceneFavorite sceneFavorites = ProfileData.Instance.GetSceneFavorites(sceneGUID);

            if (sceneFavorites == null || !sceneFavorites.HasValidValues())
            {
                EditorGUILayout.LabelField("No favorites yet - drag GameObjects here to add them.");
            }
            else
            {
                string markedForRemoval = null;

                //Copy since removal below mutates the underlying list
                foreach (string globalId in sceneFavorites.FavoriteGlobalIds.ToArray())
                {
                    //Resolved through SceneFavorite's cache - GlobalObjectId resolution is slow,
                    //so it must not run fresh on every repaint (this tab redraws at 10Hz).
                    GameObject go = sceneFavorites.ResolveGameObject(globalId);
                    if (go == null)
                        continue;

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(EditorGUIUtility.ObjectContent(go, typeof(GameObject)), EditorStyles.label, GUILayout.Height(20)))
                    {
                        Selection.activeGameObject = go;
                        EditorGUIUtility.PingObject(go);
                    }

                    if (GUILayout.Button(m_iconDelete, GUIStyle.none, GUILayout.Width(18), GUILayout.Height(18)))
                        markedForRemoval = globalId;

                    EditorGUILayout.EndHorizontal();
                }

                if (markedForRemoval != null)
                    ProfileData.Instance.RemoveSceneObjectFavorite(sceneGUID, markedForRemoval);
            }

            drawSceneFavoritesDropArea();

            EditorGUILayout.EndVertical();
        }

        private void drawSceneFavoritesDropArea()
        {
            Rect dropArea = GUILayoutUtility.GetRect(0, 30, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop GameObjects here to favorite them", EditorStyles.helpBox);

            Event e = Event.current;
            if (!dropArea.Contains(e.mousePosition))
                return;

            if (e.type == EventType.DragUpdated)
            {
                bool hasGameObject = DragAndDrop.objectReferences.Any(obj => obj is GameObject);
                DragAndDrop.visualMode = hasGameObject ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                e.Use();
            }
            else if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                {
                    //Adds to whichever scene the dropped GameObject actually belongs to,
                    //regardless of which scene's block visually received the drop.
                    if (obj is GameObject go)
                        ProfileData.Instance.AddSceneObjectFavorite(go);
                }
                e.Use();
            }
        }

        private void drawHistory()
        {
            bool showThumbNails = Preferences.ShowThumbnails;

            int showCount = Preferences.HistoryCountLimit;
            int showCounter = 0;

            GUIContent guiContentHistory = new GUIContent("Sort:History");
            guiContentHistory.tooltip = "Most recent action is shown on top";
            GUIContent guiContentFrequency = new GUIContent("Sort:Frequency");
            guiContentFrequency.tooltip = "Most used actions is shown on top";

            if (ProfileData.Instance.GetSmartHistory().Length >= 1)
            {
                ProfileData.Instance.HistorySortState = GUILayout.Toolbar(ProfileData.Instance.HistorySortState, new GUIContent[] { guiContentHistory, guiContentFrequency }, GUILayout.Height(20), GUILayout.MaxWidth(200));
            }
            else
            {
                EditorGUILayout.LabelField("No valid history yet!");
                EditorGUILayout.LabelField("- Populate history by performing actions");
            }

            foreach (string assetIDs in ProfileData.Instance.GetSmartHistory())
            {
                UIStyler.DrawAssetFromID(assetIDs, showThumbNails);
                showCounter++;
                //Break loop
                if (showCounter >= showCount)
                    break;
            }
        }

        private void drawUserFavorites()
        {
            EditorGUI.BeginChangeCheck();
            m_favoritesGroupMode = GUILayout.Toolbar(m_favoritesGroupMode, new string[] { "By Type", "By List" }, GUILayout.Height(20), GUILayout.MaxWidth(200));
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetInt(Preferences.QSPrefsFavoritesGroupMode, m_favoritesGroupMode);

            EditorGUILayout.Space();

            if (m_favoritesGroupMode == GroupModeByList)
                drawFavoritesByList();
            else
            {
                List<KeyValueFavorite> keyValues = ProfileData.Instance.GetAssetFavoritesKeyed();
                drawKeyValueFavorites(keyValues);
            }
        }

        private void drawFavoritesByList()
        {
            bool showThumbNails = Preferences.ShowThumbnails;

            EditorGUILayout.BeginHorizontal();
            m_newListName = EditorGUILayout.TextField(m_newListName);
            GUI.enabled = !string.IsNullOrEmpty(m_newListName);
            if (GUILayout.Button("+ New List", GUILayout.Width(90)))
            {
                ProfileData.Instance.CreateList(m_newListName);
                m_newListName = "";
                GUI.FocusControl(null);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            List<FavoriteList> lists = ProfileData.Instance.GetCustomLists();
            FavoriteList markedListForDelete = null;

            foreach (FavoriteList list in lists)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(list.Name + " (" + list.ItemIds.Count + ")", EditorStyles.boldLabel);

                //Right click the header to delete the whole list (items stay favorited, just unfiled)
                Event headerEvent = Event.current;
                if (headerEvent.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(headerEvent.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    FavoriteList listToDelete = list;
                    menu.AddItem(new GUIContent("Delete list"), false, () => markedListForDelete = listToDelete);
                    menu.ShowAsContext();
                    headerEvent.Use();
                }

                string markedItemForRemoval = null;
                foreach (string id in list.ItemIds.ToArray())
                {
                    EditorGUILayout.BeginHorizontal();
                    UIStyler.DrawAssetFromID(id, showThumbNails);
                    if (GUILayout.Button(m_iconDelete, GUIStyle.none, GUILayout.Width(18), GUILayout.Height(18)))
                        markedItemForRemoval = id;
                    EditorGUILayout.EndHorizontal();
                }

                if (markedItemForRemoval != null)
                    ProfileData.Instance.RemoveFromList(list, markedItemForRemoval);

                drawListDropArea(list);

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (markedListForDelete != null)
                ProfileData.Instance.DeleteList(markedListForDelete);

            EditorGUILayout.LabelField("Unlisted", EditorStyles.boldLabel);
            List<string> unlisted = ProfileData.Instance.GetUnlistedFavoriteIDs();
            if (unlisted.Count == 0)
                EditorGUILayout.LabelField("- Everything is filed into a list");
            else
                foreach (string id in unlisted)
                    UIStyler.DrawAssetFromID(id, showThumbNails);
        }

        private void drawListDropArea(FavoriteList list)
        {
            Rect dropArea = GUILayoutUtility.GetRect(0, 24, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag a favorite here to add it to this list", EditorStyles.helpBox);

            Event e = Event.current;
            if (!dropArea.Contains(e.mousePosition))
                return;

            if (e.type == EventType.DragUpdated)
            {
                bool hasValidItem = DragAndDrop.objectReferences.Any(obj => obj != null && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj)));
                DragAndDrop.visualMode = hasValidItem ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                e.Use();
            }
            else if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    string id = AssetDatabase.AssetPathToGUID(path);
                    ProfileData.Instance.AddToList(list, id);
                }
                e.Use();
            }
        }

        private void drawKeyValueFavorites(List<KeyValueFavorite> keyValues)
        {
            bool allFavoritesVisible = ProfileData.Instance.IsAllFavoriteTypesVisible();
            bool allFavoritesHidden = ProfileData.Instance.IsAllFavoriteTypesHidden();

            //Populate context menu
            GenericMenu favoriteContextFilter = new GenericMenu();
            favoriteContextFilter.AddItem(new GUIContent("ALL"), allFavoritesVisible, ProfileData.Instance.MakeAllFavoriteTypesVisible);
            favoriteContextFilter.AddItem(new GUIContent("NONE"), allFavoritesHidden, ProfileData.Instance.MakeAllFavoritesTypesHidden);

            favoriteContextFilter.AddSeparator("");

            if (keyValues != null)
                foreach (KeyValueFavorite kv in keyValues)
                {
                    favoriteContextFilter.AddItem(new GUIContent(kv.Key), !kv.IsHidden, kv.OnToggleSearchFilter);
                }

            //Draw search field
            bool filterDirty = false;
            drawSearchField(out filterDirty, favoriteContextFilter);

            bool showThumbNails = Preferences.ShowThumbnails;
            List<KeyValueFavorite> m_markedKeysForDelete = new List<KeyValueFavorite>();

            if (keyValues == null || keyValues.Count == 0)
            {
                EditorGUILayout.LabelField("No favorites added yet!");
                EditorGUILayout.LabelField("- Right click assets/folders");
                EditorGUILayout.LabelField("- Favorite  Assets -> Add to Favorites");
            }
            else if (allFavoritesHidden)
            {
                EditorGUILayout.LabelField("All types are hidden!");
                EditorGUILayout.LabelField("- Click the magnifying glass in the search bar");
                EditorGUILayout.LabelField("- Select which types to see");
            }
            else
            {
                foreach (KeyValueFavorite keyedFavorite in keyValues)
                {
                    if (filterDirty)
                        keyedFavorite.SetFavoriteFilter(m_favoriteSearchString);

                    List<string> filteredFavorites = keyedFavorite.GetFilteredFavorites();

                    if (keyedFavorite.IsHidden || filteredFavorites.Count < 1)
                        continue;

                    EditorGUILayout.BeginHorizontal();

                    //Either choose type icon, or the specific preview icon for that particular asset
                    Texture assetIcon = keyedFavorite.Icon;
                    Vector2 iconSize = new Vector2(32, 32);

                    GUILayout.Label(assetIcon, GUILayout.Width(iconSize.x), GUILayout.Height(iconSize.y));

                    EditorGUILayout.BeginVertical();

                    List<string> m_markedItemsForDelete = new List<string>();
                    foreach (string id in filteredFavorites)
                    {
                        EditorGUILayout.BeginHorizontal();

                        //If favoritte is folder
                        if (keyedFavorite.IsFolder)
                        {
                            EditorGUI.indentLevel = 0;
                            drawUIElementFolderFromID(id);
                        }
                        //If favoritte is asset
                        else
                            UIStyler.DrawAssetFromID(id, showThumbNails);

                        if (GUILayout.Button(m_iconDelete, GUIStyle.none, GUILayout.Width(18), GUILayout.Height(18)))
                            m_markedItemsForDelete.Add(id);

                        //Reset Indent
                        EditorGUI.indentLevel = 0;
                        EditorGUILayout.EndHorizontal();
                    }

                    //Delete the marked values
                    foreach (string deleteItem in m_markedItemsForDelete)
                        ProfileData.Instance.RemoveFromFavorites(deleteItem);

                    //Check if it still has any values
                    if (!keyedFavorite.HasValidValues())
                        m_markedKeysForDelete.Add(keyedFavorite);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                //Delete the marked keys
                foreach (KeyValueFavorite deleteItem in m_markedKeysForDelete)
                    keyValues.Remove(deleteItem);


                //Detect Delete event
                Event e = Event.current;
                if (e.isKey && e.type == EventType.KeyUp)
                {
                    if (e.keyCode == KeyCode.Delete)
                    {
                        if (ProfileData.Instance.ContainsFavoriteID(GUI.GetNameOfFocusedControl()))
                        {
                            ProfileData.Instance.RemoveFromFavorites(GUI.GetNameOfFocusedControl());
                        }
                    }
                }
            }
        }

        private void drawUIElementFolderFromID(string id)
        {
            bool showThumbNails = Preferences.ShowThumbnails;

            string path = AssetDatabase.GUIDToAssetPath(id);
            string foldername = path.Substring(path.IndexOf("/") + 1);

            if (!m_favoriteFolderFoldoutDict.ContainsKey(id))
                m_favoriteFolderFoldoutDict.Add(id, false);

            EditorGUILayout.BeginVertical();

            string[] children = Utils.GetImmediateChildren(path);
            //No children in folder, so no point in showing foldout
            if (children.Length < 1)
            {
                EditorGUILayout.LabelField("EMPTY FOLDER: " + path);
                UIStyler.AddGenericMenu("Remove folder from favorites", foldername);
            }
            else
            {
                GUI.SetNextControlName(id);
                m_favoriteFolderFoldoutDict[id] = EditorGUILayout.Foldout(m_favoriteFolderFoldoutDict[id], "Folder: " + foldername);
                //Allow for delete favorite menu
                UIStyler.AddGenericMenu("Remove folder from favorites", id);

                //If foldout is active
                if (m_favoriteFolderFoldoutDict[id])
                {
                    foreach (string childPath in children)
                    {
                        string assetId = AssetDatabase.AssetPathToGUID(childPath);
                        UnityEngine.Object loadedAsset = AssetDatabase.LoadMainAssetAtPath(childPath);

                        if (loadedAsset != null) // && !isFolder
                            UIStyler.DrawAssetFromID(assetId, showThumbNails);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void drawSearchField(out bool filterDirty, GenericMenu contextMenu)
        {
            EditorGUI.BeginChangeCheck();

            //Search
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            //Searchfield
            Rect position = GUILayoutUtility.GetRect(50, 250, 10, 50, EditorStyles.toolbarTextField);
            position.width -= 16;
            position.x += 16;
            m_favoriteSearchString = GUI.TextField(position, m_favoriteSearchString, EditorStyles.toolbarTextField);

            position.x = position.x - 18;
            position.width = 20;
            if (GUI.Button(position, "", GUI.skin.FindStyle(_styleTextFieldName)))
            {
                contextMenu.DropDown(position);
            }

            position = GUILayoutUtility.GetRect(10, 10, GUI.skin.FindStyle(_styleSearchCancelButton));
            position.x -= 5;
            if (GUI.Button(position, "", GUI.skin.FindStyle(_styleSearchCancelButton)))
            {
                m_favoriteSearchString = string.Empty;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //If search was changed, update filter
            filterDirty = EditorGUI.EndChangeCheck();
        }
    }
}