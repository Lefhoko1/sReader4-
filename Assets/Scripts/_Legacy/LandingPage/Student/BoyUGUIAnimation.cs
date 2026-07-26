using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoyUGUIAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameTime = 0.2f;

    private Image image;
    private int currentFrame;
    private float timer;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        if (frames != null && frames.Length > 0)
        {
            SetFrame(0);
        }
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= frameTime)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            SetFrame(currentFrame);
        }
    }

    private void SetFrame(int index)
    {
        if (frames[index] != null)
        {
            image.sprite = frames[index];
        }
    }
}
