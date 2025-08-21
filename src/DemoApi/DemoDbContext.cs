using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class DemoDbContext : DbContext
{
    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options) { }

    public DbSet<Message> Messages { get; set; } = null!;
}

[Table("messages")]
public class Message
{
    [Column("id")]
    public int Id { get; set; }

    [Column("text")]
    public string Text { get; set; } = "";
}
