using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace SReader.EditorTools
{
    /// <summary>
    /// Procedurally builds a simple chair out of editable ProBuilder cubes
    /// (seat, backrest and four legs). Run it from the menu:
    ///   Tools ▸ ProBuilder ▸ Build Chair
    ///
    /// Each part is a real ProBuilderMesh, so after generation you can keep
    /// editing it by hand with the normal ProBuilder tools (extrude faces,
    /// bevel edges, etc.). This is an Editor-only helper and is never
    /// included in a player build.
    /// </summary>
    public static class ProBuilderChairBuilder
    {
        // All measurements are in metres, matching Unity's default unit.
        private const float SeatSize = 0.45f;   // seat width / depth
        private const float SeatThickness = 0.06f;
        private const float SeatHeight = 0.45f;  // top of the legs / underside of seat

        private const float LegSize = 0.05f;     // square leg cross-section
        private const float BackHeight = 0.45f;  // how far the backrest rises above the seat
        private const float BackThickness = 0.05f;

        [MenuItem("Tools/ProBuilder/Build Chair")]
        public static void BuildChair()
        {
            // Root that holds the whole chair so it moves/selects as one object.
            var root = new GameObject("ProBuilder Chair");
            Undo.RegisterCreatedObjectUndo(root, "Build ProBuilder Chair");

            // --- Seat ---------------------------------------------------------
            var seatCenterY = SeatHeight + (SeatThickness * 0.5f);
            CreatePart(
                "Seat",
                root.transform,
                new Vector3(0f, seatCenterY, 0f),
                new Vector3(SeatSize, SeatThickness, SeatSize));

            // --- Legs ---------------------------------------------------------
            // Inset the legs slightly from the seat edges.
            var legOffset = (SeatSize * 0.5f) - (LegSize * 0.5f) - 0.02f;
            var legCenterY = SeatHeight * 0.5f;
            var legPositions = new[]
            {
                new Vector3(legOffset, legCenterY, legOffset),
                new Vector3(-legOffset, legCenterY, legOffset),
                new Vector3(legOffset, legCenterY, -legOffset),
                new Vector3(-legOffset, legCenterY, -legOffset),
            };

            for (var i = 0; i < legPositions.Length; i++)
            {
                CreatePart(
                    $"Leg {i + 1}",
                    root.transform,
                    legPositions[i],
                    new Vector3(LegSize, SeatHeight, LegSize));
            }

            // --- Backrest -----------------------------------------------------
            // Sits on the rear edge of the seat and rises upward.
            var backZ = -(SeatSize * 0.5f) + (BackThickness * 0.5f);
            var backCenterY = SeatHeight + SeatThickness + (BackHeight * 0.5f);
            CreatePart(
                "Backrest",
                root.transform,
                new Vector3(0f, backCenterY, backZ),
                new Vector3(SeatSize, BackHeight, BackThickness));

            // Select the finished chair in the hierarchy.
            Selection.activeGameObject = root;
            SceneView.lastActiveSceneView?.FrameSelected();

            Debug.Log("[ProBuilder] Chair built. Select a part and use the ProBuilder window to keep editing it.");
        }

        private static void CreatePart(string name, Transform parent, Vector3 localPosition, Vector3 size)
        {
            // GenerateCube returns a fully-formed, editable ProBuilder mesh.
            var mesh = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            mesh.gameObject.name = name;

            var t = mesh.transform;
            t.SetParent(parent, worldPositionStays: false);
            t.localPosition = localPosition;

            // Rebuild the Unity mesh + collider from the ProBuilder data.
            mesh.ToMesh();
            mesh.Refresh();

            Undo.RegisterCreatedObjectUndo(mesh.gameObject, "Build ProBuilder Chair");
        }
    }
}
