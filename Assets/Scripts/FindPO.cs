public class FindPO : PowerUp
{
    public FindPO(int charge) : base(charge) { }

    public override bool Apply(Board board , GameManager gamemanager)
    {
        foreach( Cell cell in board.Cells)
        {
            if(cell.IsSpecial && !cell.IsShowed)
                {
                    cell.Reveal();
                    gamemanager.OnCorrectChoice();
                    return true;
                }
        }
        return false;
        
    }
}