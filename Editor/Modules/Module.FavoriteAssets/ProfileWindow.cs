using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class ProfileWindow : EditorWindow
    {
        private const string WINDOWNAME = "Favorite Assets Profiles";
        List<ProfileData> m_allFavorites;
        private Vector2 m_scrollPos;
        private string m_newUserString = "";
        const int m_defaultIconSize = 20;
        private bool m_attemptToCreateUser;

        public static void Init()
        {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            ProfileWindow m_window = EditorWindow.GetWindowWithRect<ProfileWindow>(new Rect(110, 110, 300, 500), true, WINDOWNAME);
            m_window.position = new Rect(mousePos.x - 150, mousePos.y + 50, 250, 400);
            m_window.checkUserprofiles();
        }

        private void OnProjectChange()
        {
            checkUserprofiles();
        }

        private void checkUserprofiles()
        {
            m_allFavorites = new List<ProfileData>();

            //Find all the valid resource IDs
            string[] existingProfileAssetIDs = AssetDatabase.FindAssets("t:" + typeof(ProfileData), null);

            //Find their paths
            string[] existingProfileAssetPaths = new string[existingProfileAssetIDs.Length];
            for (int i = 0; i < existingProfileAssetIDs.Length; i++)
                existingProfileAssetPaths[i] = AssetDatabase.GUIDToAssetPath(existingProfileAssetIDs[i]);

            //Load the objects
            foreach (string path in existingProfileAssetPaths)
                m_allFavorites.Add(AssetDatabase.LoadAssetAtPath<ProfileData>(path));

            m_allFavorites = m_allFavorites.OrderBy(val => val.GetUserName()).ToList();
        }

        void OnGUI()
        {
            GUILayout.Label("Current active profile: " + ProfileData.Instance.GetUserName(), EditorStyles.largeLabel);
            GUILayout.Label(ConfigData.Instance.UsersIcon);
            drawNewUserField();
            GUILayout.Label("Avaliable profiles", EditorStyles.boldLabel);

            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
            foreach (ProfileData user in m_allFavorites)
            {
                bool isCurrentUser = (user == ProfileData.Instance);

                EditorGUILayout.BeginHorizontal();
                if (!isCurrentUser)
                {
                    drawUserActionBtn(user, ConfigData.Instance.DeleteIcon, "Delete \"" + user.GetUserName() + "\"", deleteAsset);
                    drawUserActionBtn(user, ConfigData.Instance.CopyIcon, "Copy favorites from \"" + user.GetUserName() + "\"", copyFrom);
                    drawUserActionBtn(user, ConfigData.Instance.SwapIcon, "Swap active profile to \"" + user.GetUserName() + "\"", swapActiveUser);
                }
                else
                    GUILayout.Space(64);

                GUILayout.Label(user.GetUserName());
                if (isCurrentUser)
                {
                    GUIContent content = new GUIContent("", ConfigData.Instance.SelectedIcon);
                    GUILayout.Label(content, GUIStyle.none, GUILayout.Width(m_defaultIconSize), GUILayout.Width(m_defaultIconSize), GUILayout.Height(m_defaultIconSize));
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void drawNewUserField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.PrefixLabel("Create new");

            string newControlName = " .NewUserNameTextField";
            GUI.SetNextControlName(newControlName);

            //REGEX FOR VALID FILENAME
            string validationRegex = @"^[\w\-. ]+$";
            bool matchesRegex = Regex.IsMatch(m_newUserString, validationRegex);
            bool startsWithSpace = m_newUserString != "" && char.IsWhiteSpace(m_newUserString, 0);

            EditorGUILayout.BeginVertical();
            m_newUserString = EditorGUILayout.TextField(m_newUserString, GUILayout.Width(150));
            //Check if valid user name
            bool validUserName = true;
            if (m_newUserString != "" && (startsWithSpace || !matchesRegex || m_allFavorites.Exists(val => val.GetUserName().ToLowerInvariant() == m_newUserString.ToLowerInvariant())))
                validUserName = false;

            if (!validUserName)
            {
                Color origColor = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Not a valid username", EditorStyles.whiteBoldLabel);
                GUI.color = origColor;
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button(ConfigData.Instance.PlusIcon, GUIStyle.none, GUILayout.Width(m_defaultIconSize), GUILayout.Height(m_defaultIconSize)))
                createNewUser(validUserName);
            else if (m_attemptToCreateUser)
            {
                createNewUser(validUserName);
                m_attemptToCreateUser = false;
            }

            EditorGUIUtility.labelWidth = 0;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            Event e = Event.current;
            //If we press enter after having written new user name
            if (e.isKey && e.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == newControlName)
            {
                this.Focus();
                m_attemptToCreateUser = true;
                GUI.FocusControl("");
                this.Repaint();
            }
            //Focus elsewhere since we click elsewhere
            if (e.isMouse && e.button == 0 && GUI.GetNameOfFocusedControl() == newControlName)
            {
                this.Focus();
                GUI.FocusControl("");
                this.Repaint();
            }
        }

        private void createNewUser(bool isValidUser)
        {
            if (isValidUser && m_newUserString != "")
            {
                Utils.CreateNewUserFavorite(m_newUserString);
                m_newUserString = "";
                GUI.FocusControl("");
                this.Repaint();
            }
        }

        private void drawUserActionBtn(ProfileData user, Texture icon, string tooltip, System.Action<ProfileData> callback)
        {
            GUIContent content = new GUIContent("", icon, tooltip);
            if (GUILayout.Button(content, GUIStyle.none, GUILayout.Width(m_defaultIconSize), GUILayout.Height(m_defaultIconSize)))
                callback(user);
        }

        private void copyFrom(ProfileData target)
        {
            if (EditorUtility.DisplayDialog("Copy user data", "This will add the favorites from \"" + target.GetUserName() + "\" to \"" + ProfileData.Instance.GetUserName() + "\"", "Copy", "Cancel"))
                ProfileData.Instance.CopyFrom(target);
        }

        private void deleteAsset(ProfileData target)
        {
            if (EditorUtility.DisplayDialog("Delete user", "Are you sure you want to delete \"" + target.GetUserName() + "\"", "Yes", "Cancel"))
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(target));
        }

        private void swapActiveUser(ProfileData target)
        {
            ProfileData.Instance.SwapActiveUser(target);
            Preferences.SetUserOverride(target.GetUserName());
        }
    }
}
