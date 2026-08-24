using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Meowgoku/Level")]
public class LevelData : SerializedScriptableObject
{
    public enum CellMark { None, Solution, Revealed }

    public struct CellInfo
    {
        public CellMark mark;
        public Color color;
    }

    public int _size = 3;

    [TableMatrix(SquareCells = true, DrawElementMethod = "DrawElement")]
    public CellInfo[,] cell;

    [Button]
    public void CreateGrid()
    {
        cell = new CellInfo[_size, _size];

        for (int y = 0; y < _size; y++)
        for (int x = 0; x < _size; x++)
            cell[x, y].color = Palette[0];

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public static readonly Color[] Palette =
    {
        Color.white,
        new Color(0.98f, 0.85f, 0.55f), // yellow
        new Color(0.72f, 0.65f, 0.92f), // purple
        new Color(0.62f, 0.45f, 0.35f), // brown
        new Color(0.55f, 0.80f, 0.55f), // green
        new Color(0.90f, 0.55f, 0.65f), // pink
        new Color(0.96f, 0.66f, 0.38f), // orange
    };

#if UNITY_EDITOR
    private const float StripHeight = 14f;
    private static GUIStyle _markStyle;

    private static CellInfo DrawElement(Rect rect, CellInfo value)
    {
        Rect strip = new Rect(rect.x + 1, rect.yMax - StripHeight - 1, rect.width - 2, StripHeight);
        Rect body  = new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - StripHeight - 3);

        Event e = Event.current;
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition) && !strip.Contains(e.mousePosition))
        {
            if (e.button == 0)
                value.mark = (CellMark)(((int)value.mark + 1) % 3);
            else if (e.button == 1)
                value.color = NextColour(value.color);

            GUI.changed = true;
            e.Use();
        }

        Color background = value.color;
        background.a = 1f;
        UnityEditor.EditorGUI.DrawRect(body, background);

        if (value.mark != CellMark.None)
        {
            if (_markStyle == null)
            {
                _markStyle = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
                _markStyle.alignment = TextAnchor.MiddleCenter;
                _markStyle.fontSize = 18;
            }

            _markStyle.normal.textColor = value.mark == CellMark.Solution ? Color.black : Color.red;
            GUI.Label(body, value.mark == CellMark.Solution ? "O" : "@", _markStyle);
        }

        // showEyedropper: true, showAlpha: false, hdr: false
        value.color = UnityEditor.EditorGUI.ColorField(strip, GUIContent.none, value.color, true, false, false);

        return value;
    }

    private static Color NextColour(Color current)
    {
        for (int i = 0; i < Palette.Length; i++)
            if (Palette[i] == current)
                return Palette[(i + 1) % Palette.Length];

        return Palette[0];
    }
#endif
}
