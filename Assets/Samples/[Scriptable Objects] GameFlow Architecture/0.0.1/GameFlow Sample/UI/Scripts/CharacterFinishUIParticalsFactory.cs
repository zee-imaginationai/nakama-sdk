using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFinishUIParticalsFactory 
{
    public static RectTransform CreateParticals(string particleName,Transform parent)
    {
        RectTransform panel = MonoBehaviour.Instantiate(Resources.Load(particleName),parent) as RectTransform;
        return panel;
    }
}
