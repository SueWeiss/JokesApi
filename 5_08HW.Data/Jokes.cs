using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class Jokes
{
    public string Setup { get; set; }
    public string Punchline { get; set; }
    [JsonIgnore]
    public int Id { get; set; }
    public List<UserLikedJokes> UsersLiked { get; set; }
   [JsonProperty("Id")]
      public int OriginId { get; set; }
}
public class User
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public int Id { get; set; }
    public List<UserLikedJokes> JokesLiked { get; set; }

}
public class UserLikedJokes
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int JokeId { get; set; }
    public Jokes Joke { get; set; }
    public DateTime DateLiked { get; set; }
    public bool Liked { get; set; }
}
public class JokesContext : DbContext
{
    private string _connectionString;
    public JokesContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Jokes> Jokes { get; set; }
    public DbSet<UserLikedJokes> UserLikedJokes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Taken from here:https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration

        //    set up composite primary key
            modelBuilder.Entity<UserLikedJokes>()
               .HasKey(ul => new { ul.JokeId, ul.UserId });

        //    set up foreign key from QuestionsTags to Questions
           modelBuilder.Entity<UserLikedJokes>()
               .HasOne(ul => ul.Joke)
                 .WithMany(j => j.UsersLiked)
                .HasForeignKey(j => j.JokeId);

        //    set up foreign key from QuestionsTags to Tags
                 modelBuilder.Entity<UserLikedJokes>()
               .HasOne(ul => ul.User)
                .WithMany(u => u.JokesLiked)
                .HasForeignKey(u => u.UserId);
        }
    }
    public class JokesContextFactory : IDesignTimeDbContextFactory<JokesContext>
    {
        public JokesContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}5_08HW"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new JokesContext(config.GetConnectionString("ConStr"));
        }
    }

