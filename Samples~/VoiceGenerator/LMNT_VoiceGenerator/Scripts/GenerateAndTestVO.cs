using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Kinogoblin.Samples;

namespace Kinogoblin.Samples
{
    public class GenerateAndTestVO : MonoBehaviour
    {
        private AudioSource _audioSource;
        private LMNTSpeechCustom _speech;
        private IEnumerator _talking;
        private IEnumerator _prefetch;
        [SerializeField] private string customPath;
        [SerializeField] private List<TextForGenerateVO> textList;
        [SerializeField] private List<AudioClip> clips;
        [SerializeField] private string testText;

        private void Awake()
        {
            Init();
        }

        public AudioSource AudioSource
        {
            set { _audioSource = value; }
            get { return _audioSource; }
        }

        [Button()]
        private void Init()
        {
            _audioSource = GetComponent<AudioSource>();
            _speech = GetComponent<LMNTSpeechCustom>();
        }
    
        [Button()]
        private void LoadAndSaveFromList()
        {
            StartCoroutine(LoadingAndSavingFromList());
        }

        [Button]
        private void UpdateTextList()
        {
            textList = new List<TextForGenerateVO>();
            foreach (var audio in clips)
            {
                var t = new TextForGenerateVO();
                t.nameFile = audio.name;
                textList.Add(t);
            }
        }
        
        private IEnumerator LoadingAndSavingFromList()
        {
            for (int i = 0; i < textList.Count; i++)
            {
                _audioSource.clip = null;
                _speech.DeleteHandler();
                _speech.dialogue = textList[i].text;
                
                StartCoroutine(_speech.Prefetch());
                if (_audioSource.clip == null)
                {
                    yield return new WaitUntil(() => _audioSource.clip != null);
                }
                _speech.SaveAudioClip(customPath,textList[i].nameFile);
                yield return null;
            }

            Debug.Log("All VO Saved");
            yield return null;
        }

        [Button]
        public void PlayTestSound()
        {
            StartCoroutine(LoadingAndPlaySound());
        }

        private IEnumerator LoadingAndPlaySound()
        {
            _audioSource.clip = null;
            _speech.DeleteHandler();
            _speech.dialogue = testText;
                
            StartCoroutine(_speech.Prefetch());
            if (_audioSource.clip == null)
            {
                yield return new WaitUntil(() => _audioSource.clip != null);
            }
            _audioSource.Play();
            yield return null;
        }
    }
    
    [Serializable]
    public class TextForGenerateVO
    {
        public string nameFile;
        public string text;
    }
}