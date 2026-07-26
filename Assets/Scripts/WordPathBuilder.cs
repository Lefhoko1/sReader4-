// ===========================================================================
//  WordPathBuilder — lays a sentence across stones along a curve.
//  (Own file: Unity needs one MonoBehaviour per file, named after the class.)
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SReader.Domains.Assignments.Models;

public class WordPathBuilder : MonoBehaviour
{
    [Header("Content")]
    [TextArea] public string sentence = "The fox lived near a quiet river";
    public string keyword = "river";

    [Header("Stone prefabs (A/B/C + Key)")]
    public GameObject stoneA, stoneB, stoneC, stoneKey;

    [Header("Path")]
    public Transform startPoint, endPoint;
    public float curve = 0.8f;
    public float stoneScale = 1f;

    [ContextMenu("Build Path")]
    public void Build()
    {
        ClearStones();
        if (startPoint == null || endPoint == null)
        { Debug.LogWarning("[WordPath] Assign start & end points."); return; }

        var words = sentence.Trim().TrimEnd('.').Split(' ');
        var variants = new[] { stoneA, stoneB, stoneC };
        Vector3 a = startPoint.position, b = endPoint.position;
        Vector3 side = Vector3.Cross((b - a).normalized, Vector3.up);

        for (int i = 0; i < words.Length; i++)
        {
            float t = words.Length == 1 ? 0.5f : i / (float)(words.Length - 1);
            Vector3 pos = Vector3.Lerp(a, b, t)
                        + side * Mathf.Sin(t * Mathf.PI * 2f) * curve;
            bool isKey = words[i].ToLower().Trim() == keyword.ToLower().Trim();
            var prefab = isKey && stoneKey != null ? stoneKey : variants[i % 3];
            if (prefab == null) continue;
            var go = Instantiate(prefab, pos,
                Quaternion.Euler(0, Random.Range(-14f, 14f), 0), transform);
            go.transform.localScale = Vector3.one * stoneScale *
                                      (isKey ? 1.25f : 1f);
            var ws = go.GetComponent<WordStone>() ?? go.AddComponent<WordStone>();
            ws.word = words[i];
            ws.isKeyword = isKey;
            ws.endsSentence = (i == words.Length - 1);
        }
        Debug.Log($"[WordPath] Built {words.Length} stones.");
    }

    /// <summary>
    /// Build from REAL assignment content: one stone per word, and every word with
    /// an activity (Define / Fill / Illustrate) becomes a Key stone carrying its
    /// <see cref="ContentToken"/>. Used by WordPathGame (the Supabase path).
    /// </summary>
    public List<WordStone> BuildFromSentence(ContentSentence sentence)
    {
        var made = new List<WordStone>();
        ClearStones();
        if (sentence == null || startPoint == null || endPoint == null)
        { Debug.LogWarning("[WordPath] Need a sentence + start & end points."); return made; }

        var words = sentence.tokens.Where(t => t.isWord).ToList();
        var variants = new[] { stoneA, stoneB, stoneC };
        Vector3 a = startPoint.position, b = endPoint.position;
        Vector3 side = Vector3.Cross((b - a).normalized, Vector3.up);

        for (int i = 0; i < words.Count; i++)
        {
            var tok = words[i];
            float t = words.Count == 1 ? 0.5f : i / (float)(words.Count - 1);
            Vector3 pos = Vector3.Lerp(a, b, t) + side * Mathf.Sin(t * Mathf.PI * 2f) * curve;

            bool isKey = tok.IsActivity;
            var prefab = isKey && stoneKey != null ? stoneKey : variants[i % 3];
            if (prefab == null) continue;

            var go = Instantiate(prefab, pos,
                Quaternion.Euler(0, Random.Range(-14f, 14f), 0), transform);
            go.transform.localScale = Vector3.one * stoneScale * (isKey ? 1.25f : 1f);

            var ws = go.GetComponent<WordStone>() ?? go.AddComponent<WordStone>();
            ws.word = tok.text;
            ws.isKeyword = isKey;
            ws.token = isKey ? tok : null;
            ws.endsSentence = (i == words.Count - 1);
            made.Add(ws);
        }
        return made;
    }

    void ClearStones()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(c); else DestroyImmediate(c);
        }
    }
}
