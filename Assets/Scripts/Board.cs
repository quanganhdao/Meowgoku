using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _spawnRoot;
    [SerializeField] private Cell _cellPrefab;

    public void Build(LevelData level)
    {
        if (level.solution == null || level.colors == null)
        {
            Debug.LogError($"{level.name}: press Create Grid on the LevelData asset first.", level);
            return;
        }

        int size = level.solution.GetLength(0);

        ClearBoard();
        SetUpGrid(size);

        for (int i = 0; i < size * size; i++)
        {
            int x = i % size;
            int y = i / size;

            Cell cell = Instantiate(_cellPrefab, _spawnRoot);
            cell.Setup(level.colors[x, y], level.solution[x, y]);
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
