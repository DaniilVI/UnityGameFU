using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Tutorial/Data")]
public class TutorialData : ScriptableObject
{
    public string id;
    public Sprite[] images;
    [TextArea(3,6)]
    public string[] texts;
}
