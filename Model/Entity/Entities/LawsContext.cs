using Microsoft.EntityFrameworkCore;

namespace Entity.Entities;

public class MQTTContext : DbContext
{
    public MQTTContext()
    {
    }

    public MQTTContext(DbContextOptions<MQTTContext> options)
        : base(options)
    {
    }

    public DbSet<SysUser> SysUser { get; set; }

    public DbSet<ModelTrain> ModelTrain { get; set; }

    public DbSet<APIVersion> APIVersion { get; set; }

    public DbSet<Message> Message { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => base.OnConfiguring(optionsBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ///
        /// Define primary key
        /// 

        modelBuilder.Entity<SysUser>()
                .HasKey(u => u.UserId);

        modelBuilder.Entity<ModelTrain>()
                .HasKey(u => u.ModelId);

        modelBuilder.Entity<Message>()
                .HasKey(u => u.MessageId);


        ///
        /// Define foreign key
        /// 
        modelBuilder.Entity<Message>()
                .HasOne<SysUser>()
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .IsRequired(false);

        modelBuilder.Entity<Message>()
                .HasOne<SysUser>()
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .IsRequired(false);
    }
}
