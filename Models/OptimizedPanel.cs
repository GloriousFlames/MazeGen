public class OptimizedPanel : Panel
{
    public OptimizedPanel()
    {
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        this.UpdateStyles();
    }
}