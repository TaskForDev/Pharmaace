namespace PharmaACE.NLP.Framework
{
    public class Refiner
    {
        public Operation Operation { get; set; }
        public string Question { get; set; }
    }

    public enum Operation
    {
        Unknown,
        Add,
        Remove,
        Replace
    }
}