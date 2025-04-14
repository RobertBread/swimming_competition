using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Lab8Csharp.model;
using NLog;

namespace Lab8Csharp.persistence
{
    public class ProbaDBRepository : IProbaRepo
    {
        private readonly CdbcUtils _dbUtils;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ProbaDBRepository(string props)
        {
            logger.Info("Initializing ProbaDBRepository...");
            _dbUtils = new CdbcUtils(props);
        }

        public Proba FindOne(long id)
        {
            string query = "SELECT id, distanta, stil FROM Proba WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Proba proba = new(reader["distanta"].ToString(), reader["stil"].ToString())
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    return proba;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding Proba by id");
            }
            return null;
        }

        public IEnumerable<Proba> FindAll()
        {
            List<Proba> probe = new();
            string query = "SELECT id, distanta, stil FROM Proba";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Proba proba = new(reader["distanta"].ToString(), reader["stil"].ToString())
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    probe.Add(proba);
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding all Proba");
            }
            return probe;
        }

        public Proba Save(Proba proba)
        {
            string query = "INSERT INTO Proba (distanta, stil) VALUES (@distanta, @stil)";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@distanta", proba.Distanta);
                cmd.Parameters.AddWithValue("@stil", proba.Stil);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    proba.Id = Convert.ToInt64(cmd.ExecuteScalar());
                    return proba;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error adding Proba");
            }
            return null;
        }

        public Proba Delete(long id)
        {
            Proba proba = FindOne(id);
            if (proba == null)
                return null;

            string query = "DELETE FROM Proba WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                return proba;
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error deleting Proba");
            }
            return null;
        }

        public Proba Update(Proba proba)
        {
            string query = "UPDATE Proba SET distanta = @distanta, stil = @stil WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@distanta", proba.Distanta);
                cmd.Parameters.AddWithValue("@stil", proba.Stil);
                cmd.Parameters.AddWithValue("@id", proba.Id);
                int affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return proba;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error updating Proba");
            }
            return null;
        }
    }
}
