namespace SSSP.Tools
{
    public interface IValueProducer<T>
    {
        T GetNextValue();
    }
}
