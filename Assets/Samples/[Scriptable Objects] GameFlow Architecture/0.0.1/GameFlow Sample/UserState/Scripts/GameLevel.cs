using UnityEngine;
using ProjectCore.Variables;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "v_GameLevel", menuName = "ProjectCore/UserState/GameLevel")]
public class GameLevel : DBInt
{
    public int maxLevel;
    [Button]
    public void IncrementLevel()
    {
        if (Value ==maxLevel)
        {
            Value = 4;
        }
        ApplyChange(1);

    }
}
