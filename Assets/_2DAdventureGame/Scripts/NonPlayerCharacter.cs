using UnityEngine;
public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;
    void Start() { ShowBubble(false); }
    public void ShowBubble(bool visible) { if (dialogueBubble != null) dialogueBubble.SetActive(visible); }
}
