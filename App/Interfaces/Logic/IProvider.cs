namespace App.Interfaces.Logic
{
    public interface IProvider<T>
    {
        T Get();
    }
}