namespace App.Logic.Operations
{
    public abstract class Operation
    {
        public bool Success { get; set; }

        public virtual void Reset()
        {
            Success = false;
        }
    }
}