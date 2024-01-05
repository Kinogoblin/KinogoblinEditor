using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kinogoblin.Runtime;
using Meta.WitAi.TTS;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace Kinogoblin.Samples
{
    public class GenerateWitVo : MonoBehaviour
    {
        [SerializeField] private string customPath = "__Project__\\Audio\\NewVO";
        [SerializeField] private string tempText;
        [SerializeField] private List<WitTextForGenerateVO> textList;
        [SerializeField] private TTSService ttsService;
        [SerializeField] private TTSSpeaker speaker;
        [SerializeField] private AudioClip _asyncClip;

        [SerializeField, Dropdown(nameof(_voiceNames)), OnValueChanged("ChangeVoiceName")]
        private string voiceName;

        private bool _isWorking;
        [SerializeField] private List<TTSClipData> _clipData;
        [SerializeField] private TTSDiskCacheSettings diskCacheSettings;
        private int _loadCount;

        private List<string> _voiceNames
        {
            get
            {
                List<string> presetIds = TtsService.GetAllPresetVoiceSettings().Select(s => s.SettingsId).ToList();
                return presetIds;
            }
        }

        private TTSService TtsService
        {
            get
            {
                if (ttsService == null)
                {
                    ttsService = FindObjectOfType<TTSService>();
                }

                return ttsService;
            }
        }


        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            ChangeVoiceName();
        }

        [Button]
        private void SpeakTempText()
        {
            if (_isWorking) return;
            speaker.SpeakQueued(tempText);
        }

        [Button]
        private void SpeakAllVOList()
        {
            if (_isWorking) return;
            StartCoroutine(SpeakAsync());
        }

        [Button]
        private void SaveListVO()
        {
            if (_isWorking) return;
            StartCoroutine(LoadAndSaveVO());
        }

        private IEnumerator SpeakAsync()
        {
            _isWorking = true;
            List<string> textToSpeak = new List<string>();
            foreach (var textForGenerateVo in textList)
            {
                textToSpeak.Add(textForGenerateVo.text);
            }
            
            Helpful.ToolDebug("Start Speak");
            
            yield return speaker.SpeakQueuedAsync(textToSpeak.ToArray());

            // Play complete clip
            if (_asyncClip != null)
            {
                speaker.AudioSource.PlayOneShot(_asyncClip);
            }
            Helpful.ToolDebug("Finish Speak");
            _isWorking = false;
        }

        private IEnumerator LoadAndSaveVO()
        {
            _isWorking = true;
            ttsService.UnloadAll();
            _loadCount = textList.Count;
            Helpful.ToolDebug("Start Load");
            foreach (var textForGenerateVo in textList)
            {
                TTSClipData ttsClipData = ttsService.Load(textForGenerateVo.text, voiceName, diskCacheSettings);
                ttsClipData.onStateChange += OnStateChange;
                _clipData.Add(ttsClipData);
                yield return null;
            }
            yield return null;
        }

        private void OnStateChange(TTSClipData clipData, TTSClipLoadState clipLoadState)
        {
            switch (clipLoadState)
            {
                case TTSClipLoadState.Unloaded:
                    break;
                case TTSClipLoadState.Preparing:
                    break;
                case TTSClipLoadState.Loaded:
                    OnStreamReady(clipData, clipData.textToSpeak);
                    break;
                case TTSClipLoadState.Error:
                    break;
            }
        }

        private void OnDownloadComplete(string obj)
        {
        }

        private void OnStreamReady(TTSClipData clipData, string voText)
        {
            Helpful.ToolDebug($"Text ready to download {voText}");
            var filepath = Path.Combine(Application.dataPath, customPath);
            var fileName = textList.FirstOrDefault(text => text.text == voText);
            SavWav.Save(filepath, fileName.nameFile, clipData.clip);
            _loadCount--;
            if (_loadCount <= 0)
            {
                FinishLoading();
            }
        }

        private void FinishLoading()
        {
            foreach (var ttsClipData in _clipData)
            {
                ttsClipData.onStateChange = null;
            }
            // Play complete clip
            if (_asyncClip != null)
            {
                speaker.AudioSource.PlayOneShot(_asyncClip);
            }
            
            Helpful.ToolDebug($"Finish Loading");
            _isWorking = false;
        }

        private void ChangeVoiceName()
        {
            speaker.VoiceID = voiceName;
        }
    }

    [Serializable]
    public class WitTextForGenerateVO
    {
        public string nameFile;
        public string text;
    }
}