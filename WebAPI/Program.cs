﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using CharacterEditor;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dirName = "CharacterEditor";
            var resourcesPath = Directory.Exists(dirName)
                ? dirName
                : "../" + dirName;

            if (!Directory.Exists(resourcesPath))
            {
                Console.WriteLine($"Directory '{dirName}' with resources is not found");
                Environment.Exit(2);
            }    

            // set resource path for charsave editor
            CharacterEditor.Resources.ResourcePath = resourcesPath;
            new SaveReader("1.13c"); // load resources

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
