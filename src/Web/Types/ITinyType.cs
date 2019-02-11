namespace Web.Types
{
    public interface ITinyType<out T>
    {
        T Value { get; }
    }
}
