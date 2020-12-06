using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Weather.Services.Interfaces;

namespace Weather.Logging
{

    public class FileLogger
    {
        private readonly string _filePath;
        private static object _lock = new object();
        public FileLogger(string path)
        {
            _filePath = path;
        }
        public void Log(string info)
        {
            lock (_lock)
            {
                File.AppendAllText(_filePath, info + Environment.NewLine);
            }
        }
    }

}