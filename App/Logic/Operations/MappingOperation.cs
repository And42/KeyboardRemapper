namespace App.Logic.Operations
{
    public class MappingOperation : Operation
    {
        public int SourceKey { get; set; }
        public int MappedKey { get; set; }

        public override void Reset()
        {
            base.Reset();

            SourceKey = 0;
            MappedKey = 0;
        }
    }
}
