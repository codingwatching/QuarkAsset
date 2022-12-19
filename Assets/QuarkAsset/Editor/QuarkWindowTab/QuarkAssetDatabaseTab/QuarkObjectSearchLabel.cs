﻿using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Quark.Editor
{
    public class QuarkObjectSearchLabel
    {
        QuarkObjectTreeView treeView;
        TreeViewState treeViewState;
        SearchField searchField;
        public QuarkObjectTreeView TreeView { get { return treeView; } }

        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            var multiColumnHeaderState = new MultiColumnHeader(QuarkEditorUtility.CreateObjectMultiColumnHeader());
            treeView = new QuarkObjectTreeView(treeViewState, multiColumnHeaderState);
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
        }
        public void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();
            DrawToolbar();
            DrawTreeView(rect);
            GUILayout.EndVertical();
        }
        void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
            GUILayout.EndHorizontal();
        }
        void DrawTreeView(Rect rect)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(rect.width * 0.62f));
            Rect viewRect = GUILayoutUtility.GetRect(32, 8192, 32, 8192);
            treeView.OnGUI(viewRect);
            GUILayout.EndVertical();
        }
    }
}
