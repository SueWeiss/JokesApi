using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace _5_08HW.Data
{
    public class Manager
    {
        private string _connectionString;
        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Jokes GetAPIJoke()
        {
            var client = new HttpClient();
            var result = JsonConvert.DeserializeObject<List<Jokes>>(client.GetStringAsync("https://official-joke-api.appspot.com/jokes/programming/random").Result);
            Jokes joke = result.FirstOrDefault();

            using (var ctx = new JokesContext(_connectionString))
            {
                bool toAdd = GetJokeWithId(joke.OriginId) == null;
                if (toAdd)
                {
                    ctx.Jokes.Add(joke);
                    ctx.SaveChanges();
                }
            }
            return joke;
        }
        public IEnumerable<Jokes> GetAllJokes()
        {
            using (var ctx = new JokesContext(_connectionString))
            {
                return ctx.Jokes.Include(j => j.UsersLiked).ToList();               
            };
        }

        public Jokes GetJokeWithId(int OriginId)
        {
            using (var ctx = new JokesContext(_connectionString))
            {
                Jokes j= ctx.Jokes.FirstOrDefault(t => t.OriginId == OriginId);
                return j;
            }
        }
        public void AddLike(Jokes joke, User user)
        {
            using (var ctx = new JokesContext(_connectionString))
            {
                ctx.UserLikedJokes.Add(new UserLikedJokes
                {
                    User = user,
                    UserId=user.Id,
                    DateLiked = DateTime.Now,
                    Joke = joke,
                   JokeId=joke.Id
                 
                });
                ctx.SaveChanges();
            }
        }
        
        public IEnumerable<UserLikedJokes> GetUsersLikes(int userid)
        {
            using (var ctx = new JokesContext(_connectionString))
            {
                return ctx.UserLikedJokes.Where(u => u.UserId == userid).ToList();
            }
        }
        public IEnumerable<UserLikedJokes> GetJokesLikes(int jokeId)
        {
            using (var ctx = new JokesContext(_connectionString))
            {
                return ctx.UserLikedJokes.Where(j => j.JokeId == jokeId).ToList();
            }
        }

        //public IEnumerable<Jokes> Get10Jokes()
        //{
        //    using (var ctx = new JokesContext(_connectionString))
        //    {
        //        return ctx.Jokes.Take(10);
        //    }
        //}
        //public IEnumerable<Jokes> AddTenJokes()
        //{
        //    var client = new JokesContext(_connectionString);
        //    var jokes = new List<Jokes>();
        //    for (int i = 0; i <= 10; i++)
        //    {
        //        jokes.Add(GetAPIJoke());
        //    }
        //    client.Jokes.AddRange(jokes.Where(i => GetJokeWithId(i.OriginId) == null));
        //    return jokes;

        //}

        #region LogIns
        public void AddUser(User user, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = hash;
            using (var context = new JokesContext(_connectionString))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null; //incorrect email
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
            {
                return null;
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            using (var context = new JokesContext(_connectionString))
            {
                return context.Users.Include(u=>u.JokesLiked).ToList().Where(e => e.Email == email).First();
            }
        }
        #endregion

    }
}

