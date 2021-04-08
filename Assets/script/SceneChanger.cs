using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneChangeToGameScene()
    {
        SceneManager.LoadScene("Game_Proto");
    }
    public void SceneChangeToEditScene()
    {
        SceneManager.LoadScene("MapEditorScene");
    }
    public void DeleteApplication()
    {
        Application.Quit();
    }
}
