using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace AtomTableDumper
{
    public class DelphiApplicationTracker
    {
        private IDictionary<string, DelphiApplication> _trackedApplicationsByName = new Dictionary<string, DelphiApplication>();
       
        public void IdentifyDelphiApplications(IEnumerable<AtomTableEntry> atomTableEntries)
        {
            Dictionary<int, string> currentProcessNamesById = Process.GetProcesses().ToDictionary(process => process.Id, process => process.ProcessName);

            foreach (AtomTableEntry atomTableEntry in atomTableEntries)
            {
                int processId;
                if (TryGetDelphiProcessId(atomTableEntry, out processId))
                {
                    string processName;
                    if (currentProcessNamesById.TryGetValue(processId, out processName))
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
        }

        private const int ProcessIdLength = 8;

        public static bool TryGetDelphiProcessId(AtomTableEntry atomTableEntry, out int processId)
        {
            processId = 0;
            if (atomTableEntry.Name.Length > ProcessIdLength &&
                (atomTableEntry.Name.StartsWith("Delphi") || 
                 atomTableEntry.Name.StartsWith("WndProcPtr") || 
                 atomTableEntry.Name.StartsWith("ControlOfs") || 
                 atomTableEntry.Name.StartsWith("DlgInstancePtr")))
            {
                string processIdString = atomTableEntry.Name.Substring(atomTableEntry.Name.Length - ProcessIdLength, ProcessIdLength);
                int.TryParse(processIdString, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out processId);
            }
            return processId != 0;
        }

        public List<DelphiApplication> GetProcesses()
        {
            return _trackedApplicationsByName.Values.ToList();
        }
    }
}
