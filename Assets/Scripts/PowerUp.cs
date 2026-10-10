public abstract class PowerUp
{
    public int Charge {get ; protected set;}
    public abstract bool Apply(Board board ,GameManager GameManager );
    protected PowerUp(int charge) => Charge = charge;

    public void Use(Board board ,GameManager GameManager )
    {
        if(Charge <= 0) return;
        if(Apply(board , GameManager)) --Charge;
    }
}