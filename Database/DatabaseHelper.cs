namespace Database
{
    public class DatabaseHelper : Manager
    {
        public DatabaseHelper() : base() { }
        public DatabaseHelper(string connectionString) : base(connectionString) { }
    }
    
    public class SqlServerDatabaseHelper : Manager
    {
        public SqlServerDatabaseHelper() : base() { }
        public SqlServerDatabaseHelper(string connectionString) : base(connectionString) { }
    }
}
