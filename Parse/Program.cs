namespace Parse
{
    public partial class Program
    {
        private static void Main(string[] args)
        {
            MainClass mainClass = new MainClass(args);
            mainClass.Parse();
        }
    }
}