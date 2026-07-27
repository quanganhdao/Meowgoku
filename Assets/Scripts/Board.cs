using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] public int _columCount = 5;
    [SerializeField] public int _rowCount = 5;
    [SerializeField] GameObject _spawnRoot;
    [SerializeField] GameObject _cellCellPrefab;
    [SerializeField] bool isTesting = false;


    void Start()
    {
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
            Instantiate(_cellCellPrefab,_spawnRoot.transform);
    }
    
    private void ClearBoard()
    {
        int count = _spawnRoot.transform.childCount;
        while(count!=0)
        {
            Destroy(_spawnRoot.transform.GetChild(count).gameObject);
            --count;
        }
    }
    

}
