using System.Collections.Generic;
using UnityEngine;

public class MarkPO : PowerUp
{
    public MarkPO(int charge) : base(charge) {}

    public override bool Apply(Board board , GameManager gamemanager)
    {
        List<int> RandomExistedCell = new List<int>();
        for(int i = 0 ; i < board.Cells.Count ; i++)
        {
            if(board.Cells[i].IsShowed && board.Cells[i].IsSpecial) RandomExistedCell.Add(i); 
        }
        if(RandomExistedCell.Count <= 0) return false;

        while (RandomExistedCell.Count > 0)
        {
            int k = Random.Range(0, RandomExistedCell.Count);
            List<int> targets = CollectedTarget(board, RandomExistedCell[k]);

            if (targets.Count > 0)
            {
                foreach (int t in targets)
                    board.Cells[t].MarkWrong();
                return true;
            }

            RandomExistedCell.RemoveAt(k);
        }
        return false;
    }
    private List<int> CollectedTarget(Board board, int si)
    {
        List<int> _collectedTarget = new List<int>();
        int ver = si %  board.size;
        int hor = si / board.size;

        for(int i = 0 ; i < board.Cells.Count ; i++)
        {
            if( i / board.size == hor || i % board.size == ver || (Mathf.Abs(i / board.size - hor) == 1 && Mathf.Abs(i % board.size - ver) == 1))
            {
                if(!board.Cells[i].IsShowed && !board.Cells[i].IsSpecial)
                    _collectedTarget.Add(i);
            }
        }
        return _collectedTarget;
    }
}

