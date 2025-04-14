using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Lab8Csharp.model;
using NLog;

namespace Lab8Csharp.persistence
{
    public class ParticipantDBRepository : IParticipantRepo
    {
        private readonly CdbcUtils _dbUtils;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ParticipantDBRepository(string props)
        {
            logger.Info("Initializing ParticipantDBRepository...");
            _dbUtils = new CdbcUtils(props);
        }

        public Participant FindOne(long id)
        {
            string query = "SELECT id, name, age FROM Participant WHERE id = @id";

            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Participant participant = new(reader["name"].ToString(), Convert.ToInt32(reader["age"]))
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    return participant;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding Participant by id");
            }
            return null;
        }

        public IEnumerable<Participant> FindAll()
        {
            List<Participant> participants = new();
            string query = "SELECT id, name, age FROM Participant";

            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Participant participant = new(reader["name"].ToString(), Convert.ToInt32(reader["age"]))
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    participants.Add(participant);
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding all Participants");
            }
            return participants;
        }

        public Participant Save(Participant participant)
        {
            string query = "INSERT INTO Participant (name, age) VALUES (@name, @age)";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@name", participant.Name);
                cmd.Parameters.AddWithValue("@age", participant.Age);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    participant.Id = Convert.ToInt64(cmd.ExecuteScalar());
                    return participant;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error adding Participant");
            }
            return null;
        }

        public Participant Delete(long id)
        {
            Participant participant = FindOne(id);
            if (participant == null)
                return null;

            string query = "DELETE FROM Participant WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                return participant;
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error deleting Participant");
            }
            return null;
        }

        public Participant Update(Participant participant)
        {
            string query = "UPDATE Participant SET name = @name, age = @age WHERE id = @id";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@name", participant.Name);
                cmd.Parameters.AddWithValue("@age", participant.Age);
                cmd.Parameters.AddWithValue("@id", participant.Id);
                int affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    return participant;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error updating Participant");
            }
            return null;
        }

        public Participant FindByNameAndAge(string name, int age)
        {
            string query = "SELECT id, name, age FROM Participant WHERE name = @name AND age = @age";
            try
            {
                using SQLiteConnection con = _dbUtils.GetConnection();
                using SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@age", age);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Participant participant = new(reader["name"].ToString(), Convert.ToInt32(reader["age"]))
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    return participant;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding Participant by name and age");
            }
            return null;
        }
    }
}
