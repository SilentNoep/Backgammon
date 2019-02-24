namespace SignalRChat.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Common;

    public partial class BackgammonContext : DbContext
    {
  
        public BackgammonContext()
        {
        }
        public BackgammonContext(string connectionName) : base(connectionName)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
