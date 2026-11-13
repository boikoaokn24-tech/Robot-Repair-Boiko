using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public PlayerController player;
    public UIHandler uiHandler;
    public float restartDelay = 3f;
    public bool ended { get; private set; }
    EnemyController[] enemies;
    void Awake() { instance = this; }
    void Start() { enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None); }
    void Update()
    {
        if (ended || player == null) return;
        if (player.health <= 0) { Finish(false); return; }
        if (enemies.Length == 0) return;
        int fixedCount = 0;
        foreach (var enemy in enemies) if (enemy != null && !enemy.isBroken) fixedCount++;
        uiHandler.SetProgress(fixedCount, enemies.Length);
        if (fixedCount == enemies.Length) Finish(true);
    }
    void Finish(bool won)
    {
        ended = true;
        if (won) uiHandler.DisplayWinScreen(); else uiHandler.DisplayLoseScreen();
        Invoke(nameof(ReloadScene), restartDelay);
    }
    public void ReloadScene() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    void OnDestroy() { if (instance == this) instance = null; }
}
