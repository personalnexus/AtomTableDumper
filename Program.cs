using System;
using System.IO;
using System.Xml.Serialization;

namespace AtomTableDumper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] commandLineArguments = Environment.GetCommandLineArgs();
            string outputFileNameTemplate = commandLineArguments.Length >= 2 ? commandLineArguments[1] : "%TEMP%\\AtomTableDumper_%COMPUTERNAME%_{0:yyyy-MM_dd-HH-mm-ss}.xml";
            string outputFileName = Environment.ExpandEnvironmentVariables(string.Format(outputFileNameTemplate, DateTime.Now));

            var atomTable = new AtomTable();
            atomTable.Load();
            
            using (var fileStream = new FileStream(outputFileName, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(AtomTable));
                serializer.Serialize(fileStream, atomTable);
            }
        }
    }
}
