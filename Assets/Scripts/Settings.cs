using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
