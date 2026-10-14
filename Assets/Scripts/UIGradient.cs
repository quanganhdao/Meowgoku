using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class UIGradient : BaseMeshEffect
{
    [SerializeField] private Color _top = Color.white;
    [SerializeField] private Color _bottom = new Color(0.7f, 0.7f, 0.7f, 1f);

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
            return;

        UIVertex v = default;
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref v, i);
            min = Mathf.Min(min, v.position.y);
            max = Mathf.Max(max, v.position.y);
        }

        float height = max - min;
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref v, i);
            float t = height > 0f ? (v.position.y - min) / height : 0f;
            // multiplied, so the Image's own color (e.g. the per-cell level color) still tints it
            v.color = (Color)v.color * Color.Lerp(_bottom, _top, t);
            vh.SetUIVertex(v, i);
        }
    }
}
