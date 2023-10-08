namespace BRoles.Models
{
    public class Operations
    {
        public Operations(string username, string cause, string concernedApi)
        {
            Id=Guid.NewGuid();
            Username = username;
          
            ConcernedApi = concernedApi;
            Cause = cause;
            DateTime = DateTime.Now;
            if (cause == "")
                Status = "Accepted";
            else Status = "Not Accepted";
        }

        public Guid Id { get; set; }
        public string ConcernedApi { get; set; }
        public DateTime DateTime { get; set; }


        public string Username { get; set; } = "";

        public string Status { get; set; }
        public string Cause { get; set; }






    }
}
