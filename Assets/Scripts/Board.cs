using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private int _columCount = 5;
    [SerializeField] private int _rowCount = 5;
    [SerializeField] GameObject _spawnRoot;
    [SerializeField] GameObject _cellCellPrefab;
    [SerializeField] bool isTesting = false;
    [SerializeField] List<int> _solution;
    private int _solutionLeft;

    void Start()
    {
        _solutionLeft = _solution.Count;
        GridSetUp();
        SpawnGrid();
    }
    private void GridSetUp()
    {
        GridLayoutGroup d = _spawnRoot.GetComponent<GridLayoutGroup>();
        if(d==null)
            return;

        d.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        d.constraintCount = _columCount;
    }
    
    private void SpawnGrid()
    {
        if(isTesting)
            return; 

        ClearBoard();
        for(int i = 0 ; i < _columCount*_rowCount; i++)
        {
            GameObject d = Instantiate(_cellCellPrefab,_spawnRoot.transform);
            Cell s = d.GetComponent<Cell>();
            if(_solution.Contains(i+1))
            {
                _solution.Remove(_solution.IndexOf(i+1));
                s.TurnOnSpecial();

                if(_solution.Count <= 0)
                    GameManager.Instance.OnWin();
            }
        } 
            
    }
    
    private void ClearBoard()
    {
        foreach(Transform child in _spawnRoot.transform)
        {
            Destroy(child.gameObject);
        }
    }
    

}
