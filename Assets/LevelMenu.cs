using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;

    public void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); 
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        for (int i = 0; i < unlockedLevel; i++)
        {
            if (i < buttons.Length) 
            {
                buttons[i].interactable = true;
            }
        }
    }

    public void OpenLevel(int levelId)
    {
        string levelName =  levelId + " Level";
        SceneManager.LoadScene(levelName);
    }
}
