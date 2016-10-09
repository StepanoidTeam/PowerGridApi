
namespace PowerGridEngine
{    
    public class IdNameModel
    {        
        public string Id { get; set;}

        public string Name { get; set; }
        
        public IdNameModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
