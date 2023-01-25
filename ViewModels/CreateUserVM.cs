namespace Meshkah.ViewModels
{
    public class CreateUserVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> Selected_Groups { get; set; }
        public List<int> Selected_Roles { get; set; }
    }
}
