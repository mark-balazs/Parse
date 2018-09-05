using System.Collections.Generic;



namespace MainLibrary
{
    public partial class Program
    {
        public class MainObject
        {
            public List<Version> Versions { get; set; }

            public MainObject()
            {
                Versions = new List<Version>();
            }

            public void AddVersion(string foo)
            {
                Version rhs = new Version(foo);
                Versions.Insert(0, rhs);
            }
        }
    }
}