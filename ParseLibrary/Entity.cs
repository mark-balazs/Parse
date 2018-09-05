namespace MainLibrary
{
    public partial class Program
    {
        public class Entity
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string URL { get; set; }
            public string RequestId { get; set; }
            public string RequestUrl { get; set; }


            public Entity(string a, string b)
            {
                Description = a;
                Id = b;
                URL = "";
                RequestId = "";
                RequestUrl = "";
            }
            public Entity()
            {
                Id = "";
                Description = "";
                URL = "";
                RequestId = "";
                RequestUrl = "";
            }
        }
    }
}