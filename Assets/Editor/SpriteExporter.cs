using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExporter
{
    [MenuItem("Tools/Export Selected Sprites")]
    static void ExportSprites()
    {
        Object[] sprites = Selection.objects;

        string outputFolder = "Assets/ExportedFrames";

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        foreach (Object obj in sprites)
        {
            Sprite sprite = obj as Sprite;

            if (sprite == null)
                continue;

            Texture2D sourceTexture = sprite.texture;

            Rect rect = sprite.rect;

            Texture2D cropped = new Texture2D(
                (int)rect.width,
                (int)rect.height);

            Color[] pixels = sourceTexture.GetPixels(
                (int)rect.x,
                (int)rect.y,
                (int)rect.width,
                (int)rect.height);

            cropped.SetPixels(pixels);
            cropped.Apply();

            byte[] png = cropped.EncodeToPNG();

            File.WriteAllBytes(
                $"{outputFolder}/{sprite.name}.png",
                png);
        }

        AssetDatabase.Refresh();

        Debug.Log("Export complete.");
    }
}