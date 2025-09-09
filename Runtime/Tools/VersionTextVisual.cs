using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kinogoblin.Runtime
{
    public class VersionTextVisual : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _text;

        protected virtual void Start()
        {
            Assert.IsNotNull(_text);
            _text.text = "Version: " + Application.version;
        }

        #region Inject

        public void InjectAllVersionTextVisual(TextMeshPro text)
        {
            InjectText(text);
        }

        public void InjectText(TextMeshPro text)
        {
            _text = text;
        }

        #endregion
    }
}
