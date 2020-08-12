namespace Mocker.Infrastructure.Contracts
{
    public interface IDataMapper<T1, T2>
    {
        T2 Map(T1 input);
    }
}