using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Meowgoku / Level")]
public class LevelData : SerializedScriptableObject
{
    public int size = 3;

    [TableMatrix(SquareCells = true)]
    public Color[,] colors;

    [TableMatrix(SquareCells = true)]
    public bool[,] solution;
    
    [Button]
    private void CreateGrid()
    {
        colors   = new Color[size, size];
        solution = new bool[size, size];
    }
}
