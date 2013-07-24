namespace calibration
{
    public interface IMatrix
    {
        int Width { get; }
        int Height { get; }
        float this[int row, int col] { get; set; }
    }
}
