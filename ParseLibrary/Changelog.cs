using System.Collections.Generic;



namespace MainLibrary
{
    public partial class Program
    {
        public class Changelog
        {
            public List<Entity> Defects { get; set; }
            public List<Entity> UserStories { get; set; }
            public Changelog()
            {
                Defects = new List<Entity>();
                UserStories = new List<Entity>();
            }

        }
    }
}