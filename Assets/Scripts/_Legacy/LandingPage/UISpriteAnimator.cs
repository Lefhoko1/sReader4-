using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 10f;

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        int frame = (int)(Time.time * fps) % frames.Length;
        img.sprite = frames[frame];
    }
}