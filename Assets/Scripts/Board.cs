using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _spawnRoot;
    [SerializeField] private Cell _cellPrefab;
    [Tooltip("Trễ giữa hai đường chéo liên tiếp khi bàn cờ hiện ra")]
    [SerializeField] private float _appearStagger = 0.035f;
    private Cell [] _cells;
    public IReadOnlyList<Cell> Cells => _cells;
    public int size {get ; private set;}
    public void Build(LevelData level)
    {
        if (level.cell == null)
        {
            Debug.LogError($"{level.name}: press Create Grid on the LevelData asset first.", level);
            return;
        }

        size = level.cell.GetLength(0);

        ClearBoard();
        SetUpGrid(size);
        _cells = new Cell[size*size];

        for (int i = 0; i < size * size; i++)
        {
            int x = i % size;
            int y = i / size;
            LevelData.CellInfo info = level.cell[x, y];

            Cell cell = Instantiate(_cellPrefab, _spawnRoot);
            _cells[i] = cell;
            cell.Setup(info.color, info.mark);
            cell.Appear((x + y) * _appearStagger);
        }
    }

    private void SetUpGrid(int columns)
    {
        GridLayoutGroup grid = _spawnRoot.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }

    private void ClearBoard()
    {
        foreach (Transform child in _spawnRoot)
            Destroy(child.gameObject);
    }
}
