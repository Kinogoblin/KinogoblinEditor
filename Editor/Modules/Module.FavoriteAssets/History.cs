using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Kinogoblin.Editor.FavoriteAssets
{
    //TODO make History global, instead of per user

    [System.Serializable]
    public class History
    {
        private int m_actionQueueMaxLength = 300;

        private int m_historySortState = 0; //O = History && 1 == frequency
        public int HistorySortState
        {
            get { return m_historySortState; }
            set
            {
                //Set new sort mode and sort list accordingly
                int oldState = m_historySortState;
                //Store state in prefs
                if (oldState != value)
                    EditorPrefs.SetInt(Preferences.QSPrefsHistorySortState, value);

                m_historySortState = value;

                if (oldState != m_historySortState)
                    updateSortedHistory();
            }
        }

        private List<string> m_sourceActionList = new List<string>();

        private string[] m_sortedActionList;

        internal void AddToHistory(string id)
        {
            m_sourceActionList.Add(id);

            //Remove at end of list
            if (m_sourceActionList.Count() >= m_actionQueueMaxLength)
                m_sourceActionList.RemoveAt(0);

            updateSortedHistory();
        }

        internal void CleanData()
        {
            //History cleanup
            for (int i = m_sourceActionList.Count - 1; i >= 0; i--)
            {
                string path = AssetDatabase.GUIDToAssetPath(m_sourceActionList[i]);
                bool fileExist = System.IO.File.Exists(path);
                if (/*String.IsNullOrEmpty(path) && */!fileExist)
                    m_sourceActionList.RemoveAt(i);
            }

            updateSortedHistory();
        }

        internal bool HasValidSmartHistory()
        {
            return (m_sortedActionList != null && m_sortedActionList.Count() >= 1);
        }

        internal string[] GetSmartHistory()
        {
            if (m_sortedActionList == null || m_sortedActionList.Length == 0)
                updateSortedHistory();

            return m_sortedActionList;
        }

        private void updateSortedHistory()
        {
            IEnumerable<IGrouping<string, string>> sortedList;

            if (m_historySortState == 1) //Sort by frequency
                sortedList = m_sourceActionList.GroupBy(i => i).OrderByDescending(group => group.Count());
            else //Sort by history
            {
                List<string> actionListCopy = m_sourceActionList.ToList();
                actionListCopy.Reverse();

                sortedList = actionListCopy.GroupBy(i => i);
            }

            m_sortedActionList = new string[sortedList.Count()];

            int counter = 0;
            foreach (var item in sortedList)
            {
                m_sortedActionList[counter] = item.Key;
                counter++;
            }

            if (Preferences.KeepHistory)
                Preferences.SaveHistory(m_sourceActionList);
        }

        internal void LoadHistory()
        {
            if (Preferences.KeepHistory)
                m_sourceActionList = Preferences.LoadHistory();
        }
    }
}