using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class BoardFitter : MonoBehaviour
{
    [Tooltip("4 vật đánh dấu ranh giới; dùng vị trí (pivot) của chúng")]
    [SerializeField] private RectTransform[] _corners = new RectTransform[4];
    [Tooltip("Thụt vào trong ranh giới bao nhiêu px")]
    [SerializeField] private float _inset = 0f;

    // edit mode: live preview while you move the corners. Play mode: Board.Build calls Fit()
    void LateUpdate()
    {
        if (!Application.isPlaying)
            Fit();
    }

    public void Fit()
    {
        RectTransform rt = (RectTransform)transform;
        RectTransform parent = rt.parent as RectTransform;
        if (parent == null || _corners == null || _corners.Length == 0)
            return;

        Vector2 min = Vector2.positiveInfinity, max = Vector2.negativeInfinity;
        foreach (RectTransform c in _corners)
        {
            if (c == null) return;
            Vector2 p = parent.InverseTransformPoint(c.position);
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }
        Vector2 area = max - min - Vector2.one * (_inset * 2f);
        if (area.x <= 0f || area.y <= 0f)
            return;

        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        int n = Mathf.Max(1, grid.constraintCount);
        Vector2 natural = new Vector2(
            grid.padding.horizontal + n * grid.cellSize.x + (n - 1) * grid.spacing.x,
            grid.padding.vertical   + n * grid.cellSize.y + (n - 1) * grid.spacing.y);

        // board is square, so the tighter side decides
        float scale = Mathf.Min(area.x / natural.x, area.y / natural.y);
        Vector2 center = (min + max) * 0.5f;
        Vector3 pos = center - (Vector2.one * 0.5f - rt.pivot) * natural * scale;
        pos.z = rt.localPosition.z;

        // only write on change, otherwise edit mode marks the scene dirty every frame
        if (rt.rect.size != natural)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, natural.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, natural.y);
        }
        if (rt.localScale != Vector3.one * scale) rt.localScale = Vector3.one * scale;
        if (rt.localPosition != pos) rt.localPosition = pos;
    }
}
