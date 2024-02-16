namespace ImportUtility.Model
{
    public class AppSettings
    {
        public ConnectionStringInfo ConnectionStrings { get; set; }

        public class ConnectionStringInfo
        {
            public string DefaultConnection { get; set; }
        }
    }
}
