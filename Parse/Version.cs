namespace Parse
{
    public partial class Program
    {
        public class Version
        {
            public string VersionId { get; set; }
            public Changelog Changelog { get; set; }

            public Version()
            {
                Changelog = new Changelog();
            }
            public Version(string rhs)
            {
                Changelog = new Changelog();
                VersionId = rhs;
            }
        }
    }
}