using UnityEngine;
using UnityEngine.UIElements;
public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    public float displayTime = 4f;
    VisualElement root, healthBar, dialogue;
    float timer;
    VisualElement endScreen;
    float fade;
    void Awake() { instance = this; }
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q("HealthBar"); dialogue = root.Q("NPCDialogue");
        var restart = root.Q<Button>("RestartButton"); if (restart != null) restart.clicked += Restart;
        var loseRestart = root.Q<Button>("LoseRestartButton"); if (loseRestart != null) loseRestart.clicked += Restart;
    }
    void Restart() { GameManager.instance.ReloadScene(); }
    void Update()
    {
        if (endScreen != null) { fade = Mathf.Min(1f, fade + Time.deltaTime); endScreen.style.opacity = fade; }
        if (timer <= 0) return;
        timer -= Time.deltaTime;
        if (timer <= 0) { dialogue.AddToClassList("hidden"); root.Q("Hint").AddToClassList("hidden"); }
    }
    public void SetHealthValue(float value) { if (healthBar != null) healthBar.style.width = Length.Percent(Mathf.Clamp01(value) * 100f); }
    public void DisplayDialogue() { dialogue.RemoveFromClassList("hidden"); timer = displayTime; }
    public void SetProgress(int count, int total) { root.Q<Label>("Progress").text = "Полагоджено: " + count + " / " + total; }
    public void ShowHint(string text) { var hint = root.Q<Label>("Hint"); hint.text = text; hint.RemoveFromClassList("hidden"); timer = displayTime; }
    void End(string name) { endScreen = root.Q(name); fade = 0; endScreen.style.opacity = 0; endScreen.RemoveFromClassList("hidden"); }
    public void DisplayWinScreen() { End("WinScreenContainer"); }
    public void DisplayLoseScreen() { End("LoseScreenContainer"); }
    void OnDestroy() { if (instance == this) instance = null; }
}
