using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class VNodeWindow : EditorWindow
    {
        private static VNodeWindow window;
        private Vector2 scrollPos;
        private string readmeContent = "";
        private bool readmeOpen = false;
        private string[] changelogFiles;
        private int selectedChangelogIndex = 0;
        private string changelogContent = "";
        private Vector2 changelogScroll;

        [MenuItem("VNode/Help window")]
        public static void OpenWindow()
        {
            // If already open, focus it
            if (window != null)
            {
                window.Focus();
                return;
            }

            // Otherwise, create and show a new one
            window = CreateWindow<VNodeWindow>("VNode");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Welcome to VNode!", EditorStyles.boldLabel);

            if (GUILayout.Button("Create VNode Graph"))
            {
                GameObject nodeObject = new("VNode Graph");
                nodeObject.AddComponent<NodeCollector>();
                Selection.activeObject = nodeObject;
                NodeCollector collector = nodeObject.GetComponent<NodeCollector>();
                NodeEditorWindow.Open(collector);
            }

            GUILayout.Space(10);

            // Toggle between README and Changelog
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("README")) readmeOpen = true;
            if (GUILayout.Button("Changelog")) readmeOpen = false;
            GUILayout.EndHorizontal();

            if (readmeOpen)
            {
                // Show README
                string readmePath = Application.dataPath + "/VNode/README.md";
                if (System.IO.File.Exists(readmePath))
                    readmeContent = System.IO.File.ReadAllText(readmePath);

                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
                GUILayout.TextArea(readmeContent, GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
            }
            else
            {
                // Show Changelog
                DrawChangelogUI();
            }
        }

        private void LoadChangelogFiles()
        {
            string changelogDir = Application.dataPath + "/VNode/ChangeLog";
            if (!System.IO.Directory.Exists(changelogDir))
                System.IO.Directory.CreateDirectory(changelogDir);

            // Get all .md or .txt files
            changelogFiles = System.IO.Directory.GetFiles(changelogDir, "*.md");
            if (changelogFiles.Length > 0)
                LoadChangelog(changelogFiles[selectedChangelogIndex]);
        }

        private void LoadChangelog(string filePath)
        {
            if (System.IO.File.Exists(filePath))
                changelogContent = System.IO.File.ReadAllText(filePath);
            else
                changelogContent = "File not found!";
        }

        private void DrawChangelogUI()
        {
            GUILayout.Label("Changelogs", EditorStyles.boldLabel);

            if (changelogFiles == null || changelogFiles.Length == 0)
            {
                GUILayout.Label("No changelogs found in /VNode/Changelogs");
                if (GUILayout.Button("Reload"))
                    LoadChangelogFiles();
                return;
            }

            // Dropdown to select which changelog to view
            string[] names = changelogFiles.Select(f => System.IO.Path.GetFileName(f)).ToArray();
            int newIndex = EditorGUILayout.Popup("Select changelog", selectedChangelogIndex, names);
            if (newIndex != selectedChangelogIndex)
            {
                selectedChangelogIndex = newIndex;
                LoadChangelog(changelogFiles[selectedChangelogIndex]);
            }

            // Scrollable area for the content
            changelogScroll = GUILayout.BeginScrollView(changelogScroll, GUILayout.Height(300));
            GUILayout.TextArea(changelogContent, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
        }
    }
}