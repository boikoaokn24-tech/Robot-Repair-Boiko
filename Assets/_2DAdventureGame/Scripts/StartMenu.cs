using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class StartMenu : MonoBehaviour
{
    public string levelName = "MyLevel";
    void OnEnable() { GetComponent<UIDocument>().rootVisualElement.Q<Button>("StartButton").clicked += StartGame; }
    void StartGame() { SceneManager.LoadScene(levelName); }
}
