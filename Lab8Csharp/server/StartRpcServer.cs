// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Configuration;
// using System.Runtime.InteropServices.JavaScript;
// using Lab8Csharp.networking.utils;
// using Lab8Csharp.persistence;
// using Lab8Csharp.server.server;
// using Lab8Csharp.services;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration.Json;
// using NLog;
// using NLog.Config;
// using ConfigurationManager = System.Configuration.ConfigurationManager;
//
//
// namespace Lab8Csharp.server
// {
//     public class StartRpcServer
//     {
//         private static int DEFAULT_PORT=55556;
//         private static String DEFAULT_IP="127.0.0.1";
//         private static readonly Logger log = LogManager.GetCurrentClassLogger();
//
//         public static void Main(string[] args)
//         {
//             String props;
//             
//                 string configFilePath = "server/NLog.config";
//                 if (File.Exists(configFilePath))
//                 {
//                     var xmlConfig = new XmlLoggingConfiguration(configFilePath);
//                     LogManager.Configuration = xmlConfig;
//                 }
//                 else
//                 {
//                     Console.WriteLine("NLog.config file not found.");
//                     return;
//                 }
//                 
//                 int port = DEFAULT_PORT;
//                 String ip = DEFAULT_IP;
//                 String portS= ConfigurationManager.AppSettings["port"];
//                 if (portS == null)
//                 {
//                     log.Debug("Port property not set. Using default value "+DEFAULT_PORT);
//                 }
//                 else
//                 {
//                     bool result = Int32.TryParse(portS, out port);
//                     if (!result)
//                     {
//                         log.Debug("Port property not a number. Using default value "+DEFAULT_PORT);
//                         port = DEFAULT_PORT;
//                         log.Debug("Portul "+port);
//                     }
//                 }
//                 String ipS=ConfigurationManager.AppSettings["ip"];
//            
//                 if (ipS == null)
//                 {
//                     log.Info("Port property not set. Using default value "+DEFAULT_IP);
//                 }
//                 log.Info("Configuration Settings for database {0}",GetConnectionStringByName("inotDB"));
//                 props = GetConnectionStringByName("inotDB");
//             
//                 var config = new ConfigurationBuilder()
//                     .SetBasePath(Directory.GetCurrentDirectory())
//                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                     .Build();
//             
//                 string connectionString = config.GetConnectionString("DefaultConnection");    
//
//             // Inițializare repo-uri
//             var userRepo = new UserDBRepository(connectionString);
//             var participantRepo = new ParticipantDBRepository(connectionString);
//             var probaRepo = new ProbaDBRepository(connectionString);
//             var inscriereRepo = new InscriereDBRepository(connectionString);
//
//             IServices serviceImpl = new ServicesImpl(userRepo, participantRepo, probaRepo, inscriereRepo);
//
//             Console.WriteLine($"Starting server on port: {port}");
//
//             AbstractServer server = new RpcConcurrentServer(port, serviceImpl);
//
//             try
//             {
//                 server.Start();
//             }
//             catch (Exception e)
//             {
//                 Console.Error.WriteLine("Error starting the server: " + e.Message);
//             }
//             finally
//             {
//                 try
//                 {
//                     server.Stop();
//                 }
//                 catch (Exception e)
//                 {
//                     Console.Error.WriteLine("Error stopping server: " + e.Message);
//                 }
//             }
//         }
//         
//         static string GetConnectionStringByName(string name)
//         {
//             // Assume failure.
//             string returnValue = null;
//
//             // Look for the name in the connectionStrings section.
//             ConnectionStringSettings settings =ConfigurationManager.ConnectionStrings[name];
//
//             // If found, return the connection string.
//             if (settings != null)
//                 returnValue = settings.ConnectionString;
//
//             return returnValue;
//         }
//     }
// }


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Grpc.Core;
using Lab8Csharp.Models;
using NLog;
using NLog.Config;
using Lab8Csharp.persistence;
using Lab8Csharp.server.server;
using Microsoft.EntityFrameworkCore;
using Org.Example.Grpc;  // namespace-ul generat de proto

namespace Lab8Csharp.server
{
    public class StartGrpcServer
    {
        private static int DEFAULT_PORT = 55556;
        private static string DEFAULT_IP = "127.0.0.1";
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            string configFilePath = "server/NLog.config";
            if (File.Exists(configFilePath))
            {
                var xmlConfig = new XmlLoggingConfiguration(configFilePath);
                LogManager.Configuration = xmlConfig;
            }
            else
            {
                Console.WriteLine("NLog.config file not found.");
                return;
            }

            int port = DEFAULT_PORT;
            string ip = DEFAULT_IP;

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<InotContextE>();
            optionsBuilder.UseSqlite(connectionString);
            var dbContext = new InotContextE(optionsBuilder.Options);
            
            var userRepo = new UserEFRepository(dbContext);
            var participantRepo = new ParticipantEFRepository(dbContext);
            var probaRepo = new ProbaDBRepository(connectionString);
            var inscriereRepo = new InscriereEFRepository(dbContext);
            

            var grpcService = new ServicesGrpcImpl(userRepo, participantRepo, probaRepo, inscriereRepo);

            
            var server = new Server
            {
                Services = { IServices.BindService(grpcService) },
                Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
            };

            try
            {
                server.Start();
                Console.WriteLine($"gRPC server started on {ip}:{port}");

                Console.WriteLine("Press any key to stop the server...");
                server.ShutdownTask.Wait();

                server.ShutdownAsync().Wait();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error starting the server: " + e.Message);
            }
        }
    }
}
