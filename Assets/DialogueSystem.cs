using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this line to import TextMeshPro

public class DialogueSystem : MonoBehaviour
{
    public GameObject dialogueBox;
    public TMP_Text dialogueText; // TextMeshPro text component
    private bool isTalking = false;

    private string[] dialogues = {
        "Hello, brave warrior!",
        "I have a task for you. Please eliminate 10 mobs.",
        "Come back to me once you complete it!"
    };

    private int dialogueIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTalking)
        {
            ShowNextDialogue();
        }
    }

    public void StartDialogue()
    {
        isTalking = true;
        dialogueBox.SetActive(true);
        ShowNextDialogue();
    }

    public void EndDialogue()
    {
        isTalking = false;
        dialogueBox.SetActive(false);
    }

    private void ShowNextDialogue()
    {
        if (dialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[dialogueIndex];
            dialogueIndex++;
        }
        else
        {
            EndDialogue();
            // Trigger the quest assignment
            // FindObjectOfType<QuestManager>().AssignQuest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartDialogue();
        }
    }
}
