using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NAKAMA_ENABLED
using ProjectCore.Integrations.Internal;
using ProjectCore.Integrations.NakamaServer.Internal;
#endif
using ProjectCore.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuView : UiPanelInAndOut
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button EmailAuthButton;

    [SerializeField] private Button SyncConflictButton;
    [SerializeField] private TextMeshProUGUI SyncConflictText;

    [SerializeField] private SyncConflictPanel SyncConflictPanel;
    
    [SerializeField] private MainMenuState MainMenuState;
    
    [SerializeField] private DBString ConflictString;
    
#if NAKAMA_ENABLED
    [SerializeField] private CloudSyncPanel CloudSyncPanel;
    [SerializeField] private NakamaStorageService UserProgressService;
#endif

    private void OnEnable()
    {
        PopulateDictionaries();
        PlayButton.onClick.AddListener(OpenViewButtonPressed);
        EmailAuthButton.onClick.AddListener(OnEmailAuthButton);
        SyncConflictButton.onClick.AddListener(OnSyncConflictButtonPressed);

#if NAKAMA_ENABLED
        SyncConflictPanel.Hide();
        CloudSyncPanel.Hide();
#endif
    }

    private void OnDisable()
    {
        PlayButton.onClick.RemoveListener(OpenViewButtonPressed);
        EmailAuthButton.onClick.RemoveListener(OnEmailAuthButton);
        SyncConflictButton.onClick.RemoveListener(OnSyncConflictButtonPressed);
    }
    
#if NAKAMA_ENABLED
    public async Task<StorageType> ShowSyncConflictPanel()
    {
        var storageType = await SyncConflictPanel.Show();
        SyncConflictPanel.Hide();
        return storageType;
    }
#endif

    public override IEnumerator Show(bool isResumed)
    {
        yield return base.Show(isResumed);
        UpdateButton();
    }

    public void UpdateButton()
    {
        string value = ConflictString.GetValue();
        try
        {
            var color = _dictionary[value];
            UpdateConflictButton(value, color);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    private void OpenViewButtonPressed()
    {
        MainMenuState.GotoGame();
    }
    
    private Dictionary<Color, string> _colorDictionary;
    private Dictionary<string, Color> _dictionary;

    private void PopulateDictionaries()
    {
        _colorDictionary = new Dictionary<Color, string>
        {
            { Color.black , "Black"},
            { Color.white , "White"},
            { new Color(1f, 0.64f, 0f, 1f), "Orange" },
            { new Color(1f, 0.4f, 0.7f, 1f), "Hot Pink" },
            { Color.red, "Red" },
            { Color.green, "Green" },
            { Color.blue, "Blue" },
            { Color.yellow, "Yellow" },
            { Color.cyan, "Cyan" },
            { Color.magenta, "Magenta" }
        };
        _dictionary = new Dictionary<string, Color>();
        foreach (var color in _colorDictionary)
        {
            _dictionary.Add(color.Value, color.Key);
        }
    }

    private void OnSyncConflictButtonPressed()
    {
        var randomValue = Random.Range(0, _colorDictionary.Count);
        var randomColor = _colorDictionary.Keys.ElementAt(randomValue);
        var text = _colorDictionary[randomColor];
        UpdateConflictButton(text, randomColor);
    }

    private void UpdateConflictButton(string text, Color color)
    {
        SyncConflictText.text = text;
        SyncConflictButton.image.color = color;
        ConflictString.SetValue(text);
#if NAKAMA_ENABLED
        UserProgressService.SaveUserProgress();
#endif

    }

    private void OnEmailAuthButton()
    {
#if NAKAMA_ENABLED
        CloudSyncPanel.Show();
#endif
    }
    
}