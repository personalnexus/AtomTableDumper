using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace AtomTableDumper
{
    public class DelphiApplicationTracker
    {
        private readonly Dictionary<string, DelphiApplication> _trackedApplicationsByName = new Dictionary<string, DelphiApplication>();
       
        public void IdentifyDelphiApplications(IEnumerable<AtomTableEntry> atomTableEntries)
        {   
            Process[] currentProcesses = Process.GetProcesses();
            Dictionary<int, string> currentProcessNamesById = currentProcesses.ToDictionary(process => process.Id, process => process.ProcessName);
            Dictionary<int, string> currentProcessNamesByThreadId = GetProcessNamesByThreadId(currentProcesses);

            foreach (AtomTableEntry atomTableEntry in atomTableEntries)
            {
                string processName;
                //
                // Atom names starting with "Delphi" end with the process ID
                // Atom names starting with "ControlOfs" end with the thread ID
                //
                if (atomTableEntry.Name.Length > IdLength &&
                    (TryGetDelphiProcessName(atomTableEntry, "Delphi", currentProcessNamesById, out processName) ||
                     TryGetDelphiProcessName(atomTableEntry, "ControlOfs", currentProcessNamesByThreadId, out processName)))
                {
                    DelphiApplication delphiApplication;
                    if (!_trackedApplicationsByName.TryGetValue(processName, out delphiApplication))
                    {
                        delphiApplication = new DelphiApplication(processName);
                        _trackedApplicationsByName.Add(processName, delphiApplication);
                    }
                    delphiApplication.AddAtom(atomTableEntry.Name);
                }
            }
        }

        private static Dictionary<int, string> GetProcessNamesByThreadId(Process[] currentProcesses)
        {
            var result = new Dictionary<int, string>();
            foreach (Process process in currentProcesses)
            {
                ProcessThreadCollection threads;
                try
                {
                    threads = process.Threads;
                }
                catch (System.SystemException)
                {
                    // process does not have an ID, is not associated with a process instance, or has exited.
                    continue;
                }
                foreach (ProcessThread thread in threads)
                {
                    if (thread.Id != 0)
                    {
                        result[thread.Id] = process.ProcessName;
                    }
                }
            }
            return result;
        }

        private const int IdLength = 8;

        private static bool TryGetDelphiProcessName(AtomTableEntry atomTableEntry, string prefix, Dictionary<int, string> processNamesbyId, out string processName)
        {
            processName = null;
            if (atomTableEntry.Name.StartsWith(prefix))
            {
                string processIdString = atomTableEntry.Name.Substring(atomTableEntry.Name.Length - IdLength, IdLength);
                int processId;
                if (int.TryParse(processIdString, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out processId))
                {
                    processNamesbyId.TryGetValue(processId, out processName);
                }
            }
            return processName != null;
        }

        public List<DelphiApplication> GetApplications()
        {
            return _trackedApplicationsByName.Values.ToList();
        }
    }
}
