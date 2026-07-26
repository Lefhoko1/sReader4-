using UnityEngine;

// Keeps a screen-space UI element positioned over a world point (for level-node
// badges floating above the 3D path). Works with a Screen Space - Overlay canvas.
[ExecuteAlways]
public class WorldAnchoredUI : MonoBehaviour
{
    public Vector3 worldPosition;
    public Camera targetCamera;

    RectTransform _rt;

    void OnEnable() { _rt = transform as RectTransform; }

    void LateUpdate()
    {
        if (_rt == null) _rt = transform as RectTransform;
        var cam = targetCamera;
        if (cam == null)
        {
            var go = GameObject.Find("WorldMapCamera");
            cam = go != null ? go.GetComponent<Camera>() : Camera.main;
            targetCamera = cam;
        }
        if (cam == null || _rt == null) return;

        Vector3 sp = cam.WorldToScreenPoint(worldPosition);
        _rt.gameObject.SetActive(sp.z > 0f);      // hide if behind camera
        _rt.position = sp;
    }
}
