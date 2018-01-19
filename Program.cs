using System;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace AtomTableDumper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] commandLineArguments = Environment.GetCommandLineArgs();
            string outputFileNameTemplate = commandLineArguments.Length >= 2 ? commandLineArguments[1] : "%TEMP%\\AtomTableDumper_%COMPUTERNAME%_{0:yyyy-MM_dd-HH-mm-ss}.xml";
            TimeSpan loopInterval = commandLineArguments.Length >= 3 ? TimeSpan.Parse(commandLineArguments[2]) : TimeSpan.Zero;

            var delphiProcessTracker = loopInterval == TimeSpan.Zero ? null : new DelphiApplicationTracker();
            var atomTable = new AtomTable(delphiProcessTracker);
            do
            {
                atomTable.Load();
                string outputFileName = Environment.ExpandEnvironmentVariables(string.Format(outputFileNameTemplate, DateTime.Now));
                using (var fileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(AtomTable));
                    serializer.Serialize(fileStream, atomTable);
                }
                Thread.Sleep(loopInterval);
            }
            while (loopInterval != TimeSpan.Zero);
        }
    }
}
