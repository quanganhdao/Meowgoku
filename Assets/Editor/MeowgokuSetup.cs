using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// One-click wiring for BoardFitter + win FX in the open scene.
// Safe to run again: it only fills what's still empty.
public static class MeowgokuSetup
{
    [MenuItem("Tools/Meowgoku/Wire Board Fitter + Win FX")]
    private static void Wire()
    {
        Board board = Object.FindFirstObjectByType<Board>(FindObjectsInactive.Include);
        GameManager gm = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
        if (board == null || gm == null)
        {
            Debug.LogError("Board or GameManager not found in the open scene");
            return;
        }

        // BoardFitter lives next to the GridLayoutGroup (Board._spawnRoot)
        Transform spawnRoot = (Transform)new SerializedObject(board).FindProperty("_spawnRoot").objectReferenceValue;
        if (spawnRoot != null && !spawnRoot.TryGetComponent(out BoardFitter _))
            Undo.AddComponent<BoardFitter>(spawnRoot.gameObject);

        SerializedObject so = new SerializedObject(gm);

        SerializedProperty fx = so.FindProperty("_winFx");
        if (fx.objectReferenceValue == null)
            fx.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/FX/BG_FX_Sparkle.prefab");

        SerializedProperty root = so.FindProperty("_winFxRoot");
        GameObject resultCanvas = (GameObject)so.FindProperty("_gameResultCanvas").objectReferenceValue;
        if (root.objectReferenceValue == null && resultCanvas != null)
        {
            GameObject go = new GameObject("WinFxRoot", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "Create WinFxRoot");
            go.transform.SetParent(resultCanvas.transform, false);
            go.transform.SetAsFirstSibling();   // first child = drawn behind the result panel
            root.objectReferenceValue = go.transform;
        }

        so.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        Debug.Log($"Wired: BoardFitter on {(spawnRoot ? spawnRoot.name : "-")}, " +
                  $"Win Fx = {fx.objectReferenceValue?.name ?? "-"}, Win Fx Root = {root.objectReferenceValue?.name ?? "- (no _gameResultCanvas)"}");
    }

    private const string FlyGlowPath = "Assets/Prefab/FX/FlyGlow.prefab";

    // BG_FX_Sparkle is a full-screen ambient field, so as a fly FX it just looks like the background.
    // This builds a single glowing dot (Flare6 + additive) and assigns it as Fly Fx.
    [MenuItem("Tools/Meowgoku/Create Fly Glow + fix FX refs")]
    private static void CreateFlyGlow()
    {
        GameManager gm = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
        if (gm == null)
        {
            Debug.LogError("GameManager not found in the open scene");
            return;
        }

        GameObject temp = new GameObject("FlyGlow", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.RawImage));
        ((RectTransform)temp.transform).sizeDelta = new Vector2(140f, 140f);
        UnityEngine.UI.RawImage img = temp.GetComponent<UnityEngine.UI.RawImage>();
        img.texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GUI_Picks/FX/_Deps/Flare6.png");
        img.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefab/FX/Sparkle_Additive.mat");
        img.color = new Color(1f, 0.9f, 0.6f, 1f);
        img.raycastTarget = false;
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, FlyGlowPath);
        Object.DestroyImmediate(temp);

        SerializedObject so = new SerializedObject(gm);
        so.FindProperty("_flyFx").objectReferenceValue = prefab;

        // Win Fx must be the prefab asset, not a scene copy (the scene one had scale 0)
        SerializedProperty win = so.FindProperty("_winFx");
        if (win.objectReferenceValue is GameObject w && !EditorUtility.IsPersistent(w))
            win.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/FX/BG_FX_Sparkle.prefab");

        so.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        Debug.Log($"Fly Fx = {FlyGlowPath}, Win Fx = {win.objectReferenceValue?.name ?? "-"}. Ctrl+S to save the scene.", prefab);
    }

    private const string CellPath = "Assets/Prefab/CellTest1.prefab";

    // ButtonJuice stays on the root (it must receive the pointer events alongside Cell),
    // but squishes a stretched "Visual" child so it can't fight Cell's Appear/Punch on the root.
    [MenuItem("Tools/Meowgoku/Add Button Juice to Cell")]
    private static void AddCellJuice()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(CellPath);
        try
        {
            Transform visual = root.transform.Find("Visual");
            if (visual == null)
            {
                // snapshot first: reparenting while iterating shifts the indices
                Transform[] children = new Transform[root.transform.childCount];
                for (int i = 0; i < children.Length; i++)
                    children[i] = root.transform.GetChild(i);

                RectTransform rt = new GameObject("Visual", typeof(RectTransform)).GetComponent<RectTransform>();
                rt.SetParent(root.transform, false);
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;

                // appended in original order, so the draw order is unchanged
                foreach (Transform child in children)
                    child.SetParent(rt, false);
                visual = rt;
            }

            if (!root.TryGetComponent(out ButtonJuice juice))
                juice = root.AddComponent<ButtonJuice>();
            SerializedObject so = new SerializedObject(juice);
            so.FindProperty("_target").objectReferenceValue = visual;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, CellPath);
            Debug.Log($"Cell juice: ButtonJuice on {root.name}, target = Visual ({visual.childCount} children)",
                      AssetDatabase.LoadAssetAtPath<GameObject>(CellPath));
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
