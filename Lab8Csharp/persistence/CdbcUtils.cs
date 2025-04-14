using System;
using System.Collections.Generic;
using System.Data.SQLite;
using NLog;

namespace Lab8Csharp.persistence;

public class CdbcUtils
{
    // private readonly Dictionary<string, string> _dbProps;
    private string _dbProps;
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private SQLiteConnection _connection;

    public CdbcUtils(string props)
    {
        _dbProps = props;
    }

    private SQLiteConnection GetNewConnection()
    {
        logger.Trace("Trying to connect to database...");

        // Conexiunea pentru SQLite nu necesită server, doar fișierul bazei de date
        string connectionString = $"Data Source={_dbProps};Version=3;";

        try
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            logger.Info("Database connection successful.");
            return con;
        }
        catch (SQLiteException e)
        {
            logger.Error(e, "Error getting connection");
            throw;
        }
    }

    public SQLiteConnection GetConnection()
    {
        // NU MAI ȚINE CONEXIUNEA ÎNTR-O VARIABILĂ DE INSTANȚĂ!
        return GetNewConnection();
    }
}