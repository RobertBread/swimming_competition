// using System;
// using System.IO;
// using System.Windows.Forms;
// using System.Configuration;
// using Lab8Csharp.client.gui;
// using Lab8Csharp.networking.protocol;
// using Lab8Csharp.services;
// using Microsoft.Extensions.Configuration;
// using NLog;
// using ConfigurationManager = System.Configuration.ConfigurationManager;
//
// namespace Lab8Csharp.client
// {
//     static class StartRpcClient
//     {
//         private static readonly int DEFAULT_PORT = 55556;
//         private static readonly string DEFAULT_IP = "localhost";
//         private static readonly Logger log = LogManager.GetCurrentClassLogger();
//
//         [STAThread]
//         static void Main()
//         {
//             Application.EnableVisualStyles();
//             Application.SetCompatibleTextRenderingDefault(false);
//             
//
//             // Încarcă configurația (echivalentul client.properties)
//             var configuration = LoadConfiguration();
//
//             int port = DEFAULT_PORT;
//             String ip = DEFAULT_IP;
//             String portS= ConfigurationManager.AppSettings["port"];
//             if (portS == null)
//             {
//                 log.Debug("Port property not set. Using default value "+DEFAULT_PORT);
//             }
//             else
//             {
//                 bool result = Int32.TryParse(portS, out port);
//                 if (!result)
//                 {
//                     log.Debug("Port property not a number. Using default value "+DEFAULT_PORT);
//                     port = DEFAULT_PORT;
//                     log.Debug("Portul "+port);
//                 }
//             }
//             String ipS=ConfigurationManager.AppSettings["ip"];
//            
//             if (ipS == null)
//             {
//                 log.Info("Port property not set. Using default value "+DEFAULT_IP);
//             }
//
//             Console.WriteLine($"Connecting to server {ip}:{port}");
//
//             IServices server = new ServicesRpcProxy(ip, port);
//
//             // Inițializează login-ul
//             var loginForm = new LoginForm();
//             loginForm.SetService(server);
//
//             Application.Run(loginForm);
//         }
//
//         private static IConfiguration LoadConfiguration()
//         {
//             var builder = new ConfigurationBuilder()
//                 .SetBasePath(Directory.GetCurrentDirectory())
//                 .AddIniFile("client.properties", optional: true, reloadOnChange: false);
//
//             return builder.Build();
//         }
//     }
// }