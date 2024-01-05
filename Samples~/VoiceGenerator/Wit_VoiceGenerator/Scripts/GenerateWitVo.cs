using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kinogoblin.Runtime;
using Meta.WitAi;
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

        [SerializeField, Dropdown(nameof(CHARACTER_IDS)), OnValueChanged("ChangeVoiceName")]
        private string characterType;

        [SerializeField, Dropdown(nameof(ENVIRONMENT_IDS)), OnValueChanged("ChangeVoiceName")]
        private string environmentType;

        private bool _isWorking;
        [SerializeField] private List<TTSClipData> _clipData;
        [SerializeField] private TTSDiskCacheSettings diskCacheSettings;
        private int _loadCount;


        // Supported IDs
        private const string NONE_ID = "NONE";
        private static readonly string[] CHARACTER_IDS = new[] { NONE_ID, "CHIPMUNK", "MONSTER", "ROBOT", "DAEMON" };

        private static readonly string[] ENVIRONMENT_IDS = new[]
            { NONE_ID, "REVERB", "ROOM", "PHONE", "PA", "CATHEDRAL" };


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
            _clipData.Clear();
            ttsService.UnloadAll();
            _loadCount = textList.Count;
            Helpful.ToolDebug("Start Load");
            UpdateText(out var append,out var prepend);
            speaker.VoiceSettings.AppendedText = append.ToString();
            speaker.VoiceSettings.PrependedText = prepend.ToString();
            // ttsService.Events.Download.OnDownloadSuccess.AddListener(OnClipDownloaded);
            foreach (var textForGenerateVo in textList)
            {
                var phrase =textForGenerateVo.text;
                TTSClipData ttsClipData = ttsService.Load(phrase, speaker.VoiceSettings, diskCacheSettings);
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
                    OnClipDownloaded(clipData,clipData.textToSpeak);
                    break;
                case TTSClipLoadState.Error:
                    break;
            }
        }

        private void OnClipDownloaded(TTSClipData clipData, string voText)
        {
            Debug.Log($"Text ready to download {voText}");
            
            var filepath = Path.Combine(Application.dataPath, customPath);
            var fileName = textList.FirstOrDefault(text => voText.Contains(text.text));
            SavWav.Save(filepath,fileName.nameFile,clipData.clip);

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

        private void SaveClip(TTSClipData runtimeCachedClip)
        {
            Helpful.ToolDebug($"Text ready to save {runtimeCachedClip.textToSpeak}");
            var filepath = Path.Combine(Application.dataPath, customPath);
            var fileName = textList.FirstOrDefault(text => text.text == runtimeCachedClip.textToSpeak);
            SavWav.Save(filepath, fileName.nameFile, runtimeCachedClip.clip);
        }

        private void ChangeVoiceName()
        {
            speaker.VoiceID = voiceName;
            RefreshSsml();
        }

        private void RefreshSsml()
        {
            if (!speaker)
            {
                Helpful.ToolDebug("No speaker found");
                return;
            }

            UpdateText(out var append,out var prepend);

            // Set SSML
            speaker.PrependedText = prepend.ToString();
            speaker.AppendedText = append.ToString();
        }

        private void UpdateText(out StringBuilder append, out StringBuilder prepend)
        {
            // Get SSMLs
            prepend = new StringBuilder();
            append = new StringBuilder();

            // Get character & environment ids
            string characterId = characterType;
            if (string.Equals(characterId, NONE_ID))
            {
                characterId = null;
            }

            string environmentId = environmentType;
            if (string.Equals(environmentId, NONE_ID))
            {
                environmentId = null;
            }

            // Add sfx tag
            bool hasCharacter = !string.IsNullOrEmpty(characterId);
            bool hasEnvironment = !string.IsNullOrEmpty(environmentId);
            if (hasCharacter || hasEnvironment)
            {
                // Add ssml tags
                prepend.Append("<speak>");
                append.Append("</speak>");

                // Add prefix & postfix
                prepend.Append("<sfx");
                append.Insert(0, "</sfx>");

                // Add character
                if (hasCharacter)
                {
                    prepend.Append($" character=\"{characterId.ToLower()}\"");
                }

                // Add environment
                if (hasEnvironment)
                {
                    prepend.Append($" environment=\"{environmentId.ToLower()}\"");
                }

                // Finalize
                prepend.Append(">");
            }
        }
    }

    [Serializable]
    public class WitTextForGenerateVO
    {
        public string nameFile;
        public string text;
    }
}