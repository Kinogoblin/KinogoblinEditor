using UnityEngine;
using UnityEditor;

namespace Kinogoblin.Editor.FavoriteAssets
{
	[InitializeOnLoad]
	public static class DragHandler
	{
		private static bool isDragging = false;
		private static UnityEngine.Object[] draggedObjects;

		private static DragAndDropVisualMode m_dragVisualMode;
		private static DragAndDropVisualMode m_tmpVisualDragMode;
		private static int m_delayedVisualStyleChange;

		//This class was meant to catch all dragperform events and turn that into a sorted list of what is being used most at any given time

		static DragHandler()
		{
			if (ProfileData.Instance.dragHanglerEnable)
			{
				EditorApplication.update += OnEditorUpdateDelegate;
			}
			else
			{
				EditorApplication.update -= OnEditorUpdateDelegate;
			}
		}

		private static void OnEditorUpdateDelegate()
		{
			if (!isDragging && DragAndDrop.objectReferences.Length >= 1)
			{
				isDragging = true;
				draggedObjects = DragAndDrop.objectReferences;
			}
			else if (isDragging && DragAndDrop.objectReferences.Length < 1)
			{
				//If visualmode None, then its not allowed to drop
				if (m_dragVisualMode != DragAndDropVisualMode.None)
				{
					if (EditorWindow.focusedWindow.ToString() != "UnityEditor.ProjectBrowser")
					{
						foreach (UnityEngine.Object draggedObj in draggedObjects)
						{
							if (draggedObj == null)
								continue;

							bool isAsset = AssetDatabase.Contains(draggedObj);
							if (isAsset)
							{
								string path = AssetDatabase.GetAssetPath(draggedObj);
								bool isDirectory = AssetDatabase.IsValidFolder(path);
								if (!isDirectory)
								{
									string id = AssetDatabase.AssetPathToGUID(path);
									ProfileData.Instance.AddToHistory(id);
								}
							}
						}
					}
				}
				/*else
                    Debug.LogWarning("Cant add to history since DragAndDropVisualMode was None");*/

				m_dragVisualMode = DragAndDropVisualMode.None;
				isDragging = false;
				draggedObjects = null;
			}

			if (isDragging)
			{
				m_tmpVisualDragMode = DragAndDrop.visualMode;

				//Adding delay on a couple of frames becuase the timing between DRAGANDDROP events and EditorApplication.update callbacks are not synced. Ugly, but what are you gonna do?
				if (m_tmpVisualDragMode != m_dragVisualMode)
					m_delayedVisualStyleChange++;

				//Waiting for 2 frames
				if (m_delayedVisualStyleChange > 2)
				{
					m_dragVisualMode = DragAndDrop.visualMode;
					m_delayedVisualStyleChange = 0;
				}
			}
		}
	}
}