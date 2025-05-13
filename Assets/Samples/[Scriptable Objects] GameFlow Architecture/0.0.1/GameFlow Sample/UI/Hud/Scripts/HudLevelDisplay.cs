using TMPro;
using UnityEngine;
using ProjectCore.Variables;

public class HudLevelDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    //[SerializeField] private TextMeshProUGUI _areaNameText;

    [SerializeField] private Int GameLevel;
    //[SerializeField] private CheckpointDataList CheckpointDataList;


    private void OnEnable()
    {
        _levelNumberText.text = "LEVEL " + GameLevel.GetValue();
        //_areaNameText.text = CheckpointDataList.GetCheckpointWorldMapData(GameLevel.GetValue()).name;
    }
}
