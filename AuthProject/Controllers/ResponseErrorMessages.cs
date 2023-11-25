namespace AuthProject.Controllers
{
    public class ResponseErrorMessages
    {
        public ResponseErrorMessages()
        {
            Messages = new();
        }

        public List<string> Messages { get; set; }
    }
}
