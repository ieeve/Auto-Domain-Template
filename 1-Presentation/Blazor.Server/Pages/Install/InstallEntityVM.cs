namespace Blazor.Server.Pages.Install
{
    public class InstallEntityVM
    {
        public string Title { get; set; }
        public string EntityName { get; set; }
        public bool IsCreate { get; set; }
        public Type EntityType { get; set; }
    }
}
