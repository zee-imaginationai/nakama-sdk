using UnityEngine;
using UnityEngine.UI;

using ProjectCore.Variables;
using TMPro;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private RectTransform PotCharacter;
    [SerializeField] private Image LoadingBarImage;
    [SerializeField] private Image LoadingBackgroundImage;

    [SerializeField] private Float SceneLoadingProgress;
    [SerializeField] private Float SdkLoadingProgress;
    [SerializeField] private TextMeshProUGUI VersionCodeText;
    [Header("Themes")]

    private float _progess;
    private float _newProgress;
    private float _loadValue;

    public void HideLoading ()
    {
        Destroy(gameObject, 0.5f);
    }

    private void Start()
    {

        _loadValue = LoadingBarImage.fillAmount;
        // Sprite bgSprite =WorldMapData.GetZoneSplashSprite();
        // if (bgSprite != null)
        //     LoadingBackgroundImage.sprite = bgSprite;
        VersionCodeText.text = "v"+Application.version;
    }

    private void Update()
    {
        _newProgress = Mathf.Clamp((SceneLoadingProgress.GetValue() + SdkLoadingProgress.GetValue())/2.0f, 0.0f, 1.0f);
        if (_progess < _newProgress)
        {
            _progess = _newProgress;
            LoadingBarImage.fillAmount = _progess;
            float potNewXDelta = (_progess * 360) - (_loadValue * 360);
            // PotCharacter.anchoredPosition = new Vector2(PotCharacter.anchoredPosition.x + potNewXDelta, PotCharacter.anchoredPosition.y);
            _loadValue = _progess;
        }   
    }
}
