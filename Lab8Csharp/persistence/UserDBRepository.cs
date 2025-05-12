using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Lab8Csharp.model;
using NLog;

namespace Lab8Csharp.persistence
{
    public class UserDBRepository : IUserRepo
    {
        private readonly CdbcUtils _dbUtils;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UserDBRepository(string props)
        {
            logger.Info("Initializing UserDBRepository...");
            _dbUtils = new CdbcUtils(props);
        }

        public model.User FindOne(long id)
        {
            string query = "SELECT id, username, hashedPassword FROM User WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.User user = new(reader["username"].ToString(), reader["hashedPassword"].ToString())
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    return user;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding User by id");
            }
            return null;
        }

        public model.User FindByUsername(string username)
        {
            string query = "SELECT id, username, hashedPassword FROM User WHERE username = @username";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.User user = new model.User(reader["username"].ToString(), reader["hashedPassword"].ToString())
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    return user;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding User by username");
            }
            return null;
        }

        public IEnumerable<model.User> FindAll()
        {
            List<model.User> users = new();
            string query = "SELECT id, username, hashedPassword FROM User";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.User user = new(reader["username"].ToString(), reader["hashedPassword"].ToString())
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    users.Add(user);
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding all Users");
            }
            return users;
        }

        public model.User Save(model.User user)
        {
            string query = "INSERT INTO User (username, hashedPassword) VALUES (@username, @hashedPassword)";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@hashedPassword", user.HashedPassword);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    user.Id = Convert.ToInt64(cmd.ExecuteScalar());
                    return user;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error adding User");
            }
            return null;
        }

        public model.User Delete(long id)
        {
            model.User user = FindOne(id);
            if (user == null)
                return null;

            string query = "DELETE FROM User WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                return user;
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error deleting User");
            }
            return null;
        }

        public model.User Update(model.User user)
        {
            string query = "UPDATE User SET username = @username, hashedPassword = @hashedPassword WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@hashedPassword", user.HashedPassword);
                cmd.Parameters.AddWithValue("@id", user.Id);
                int affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return user;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error updating User");
            }
            return null;
        }
    }
}
