using UnityEngine;
using UnityEngine.UI;

public class RandomBackgroundUI : MonoBehaviour
{
    public Sprite[] backgrounds;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();

        if (backgrounds.Length == 0) return;

        if (BackgroundData.Instance.selectedBackgroundID >= 0)
        {
            int id = BackgroundData.Instance.selectedBackgroundID;
            if (id < backgrounds.Length)
                image.sprite = backgrounds[id];

            return;
        }

        int randomID = Random.Range(0, backgrounds.Length);
        BackgroundData.Instance.selectedBackgroundID = randomID;

        image.sprite = backgrounds[randomID];
    }
}
