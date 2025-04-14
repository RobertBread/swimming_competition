using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Lab8Csharp.model;
using NLog;

namespace Lab8Csharp.persistence
{
    public class InscriereDBRepository : IInscriereRepo
    {
        private readonly CdbcUtils _dbUtils;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public InscriereDBRepository(string props)
        {
            logger.Info("Initializing InscriereDBRepository...");
            _dbUtils = new CdbcUtils(props);
        }

        public Inscriere FindOne(long id)
        {
            string query = "SELECT i.id, p.id AS participant_id, p.name, p.age, pr.id AS proba_id, pr.distanta, pr.stil " +
                           "FROM Inscriere i " +
                           "JOIN Participant p ON i.participant_id = p.id " +
                           "JOIN Proba pr ON i.proba_id = pr.id " +
                           "WHERE i.id = @id";

            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var participant = new Participant(reader["name"].ToString(), Convert.ToInt32(reader["age"]))
                    {
                        Id = Convert.ToInt64(reader["participant_id"])
                    };
                    var proba = new Proba(reader["distanta"].ToString(), reader["stil"].ToString())
                    {
                        Id = Convert.ToInt64(reader["proba_id"])
                    };
                    var inscriere = new Inscriere(participant, proba)
                    {
                        Id = id
                    };
                    return inscriere;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding Inscriere by id");
            }
            return null;
        }

        public IEnumerable<Inscriere> FindAll()
        {
            var inscrieri = new List<Inscriere>();
            string query = "SELECT i.id, p.id AS participant_id, p.name, p.age, pr.id AS proba_id, pr.distanta, pr.stil " +
                           "FROM Inscriere i " +
                           "JOIN Participant p ON i.participant_id = p.id " +
                           "JOIN Proba pr ON i.proba_id = pr.id";

            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var participant = new Participant(reader["name"].ToString(), Convert.ToInt32(reader["age"]))
                    {
                        Id = Convert.ToInt64(reader["participant_id"])
                    };
                    var proba = new Proba(reader["distanta"].ToString(), reader["stil"].ToString())
                    {
                        Id = Convert.ToInt64(reader["proba_id"])
                    };
                    var inscriere = new Inscriere(participant, proba)
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    inscrieri.Add(inscriere);
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding all Inscrieri");
            }
            return inscrieri;
        }

        public Inscriere Save(Inscriere inscriere)
        {
            string query = "INSERT INTO Inscriere (participant_id, proba_id) VALUES (@participant_id, @proba_id)";
            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@participant_id", inscriere.Participant.Id);
                cmd.Parameters.AddWithValue("@proba_id", inscriere.Proba.Id);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    inscriere.Id = Convert.ToInt64(cmd.ExecuteScalar());
                    return inscriere;
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error adding Inscriere");
            }
            return null;
        }

        public Inscriere Delete(long id)
        {
            var inscriere = FindOne(id);
            if (inscriere == null)
                return null;

            string query = "DELETE FROM Inscriere WHERE id = @id";
            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                return inscriere;
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error deleting Inscriere");
            }
            return null;
        }

        public Inscriere Update(Inscriere inscriere)
        {
            string query = "UPDATE Inscriere SET participant_id = @participant_id, proba_id = @proba_id WHERE id = @id";
            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@participant_id", inscriere.Participant.Id);
                cmd.Parameters.AddWithValue("@proba_id", inscriere.Proba.Id);
                cmd.Parameters.AddWithValue("@id", inscriere.Id);
                int affected = cmd.ExecuteNonQuery();
                if (affected > 0)
                    return inscriere;
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error updating Inscriere");
            }
            return null;
        }

        public int CountParticipantsByProba(long probaId)
        {
            string query = "SELECT COUNT(*) AS participant_count FROM Inscriere WHERE proba_id = @proba_id";
            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@proba_id", probaId);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error counting participants for Proba");
            }
            return 0;
        }

        public List<Participant> FindParticipantsByProba(long probaId)
        {
            var participants = new List<Participant>();
            string query = "SELECT p.id, p.name, p.age FROM Inscriere i " +
                           "JOIN Participant p ON i.participant_id = p.id " +
                           "WHERE i.proba_id = @proba_id";

            try
            {
                using var con = _dbUtils.GetConnection();
                using var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@proba_id", probaId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var participant = new Participant(reader["name"].ToString(), Convert.ToInt32(reader["age"]))
                    {
                        Id = Convert.ToInt64(reader["id"])
                    };
                    participants.Add(participant);
                }
            }
            catch (SQLiteException e)
            {
                logger.Error(e, "Error finding participants for Proba");
            }

            return participants;
        }
    }
}
