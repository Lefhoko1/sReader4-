using UnityEngine;
using UnityEngine.UIElements;

public class ReadingBoyAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameTime = 0.2f;

    private VisualElement character;
    private int currentFrame;
    private float timer;

    private void Start()
    {
        UIDocument document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("UIDocument not found.");
            return;
        }

        character = document.rootVisualElement.Q<VisualElement>("reading-character");

        if (character == null)
        {
            Debug.LogError("VisualElement 'reading-character' not found.");
            return;
        }

        if (frames.Length > 0)
        {
            SetFrame(0);
        }
    }

    private void Update()
    {
        if (character == null)
            return;

        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= frameTime)
        {
            timer = 0f;

            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                currentFrame = 0;
            }

            SetFrame(currentFrame);
        }
    }

    private void SetFrame(int index)
    {
        if (frames[index] == null)
            return;

        character.style.backgroundImage =
            new StyleBackground(frames[index].texture);
    }
}