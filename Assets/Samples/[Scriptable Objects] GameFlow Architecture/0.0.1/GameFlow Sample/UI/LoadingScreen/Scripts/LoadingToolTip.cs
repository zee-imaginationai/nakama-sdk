using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private string[] _toolTips;

    void Start()
    {
        int randomTextIndex = Random.Range(0, _toolTips.Length);
        _tooltipText.text = _toolTips[randomTextIndex];
    }

}
