using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AtomTableDumper;

namespace AtomTableDumperTest
{
    [TestClass]
    public class DelphiApplicationTrackerTest
    {
        private Dictionary<int, string> _processNamesByProcessId;
        private Dictionary<int, string> _processNamesByThreadId;
        private List<AtomTableEntry> _atoms;
        private DelphiApplicationTracker _delphiApplicationTracker;
        
        [TestInitialize]
        public void Initialize()
        {
            _processNamesByProcessId = new Dictionary<int, string>
            {
                { 4, "System" },
                { 8, "DelphiApp1" },
                { 12, "DelphiApp2" },
                { 116, "AnotherApp" },
            };
            _processNamesByThreadId = new Dictionary<int, string>
            {
                { 1288, "DelphiApp1" },
            };
            _atoms = new List<AtomTableEntry>
            {
                new AtomTableEntry { Index = "AC", Name = "Delphi00000008" },
                new AtomTableEntry { Index = "D2", Name = "ControlOfs00000508" },
                new AtomTableEntry { Index = "842", Name = "SomethingElse" },
            };
            _delphiApplicationTracker = new DelphiApplicationTracker();
        }

        [TestMethod]
        public void IdentifyDelphiApplicationsByThreadAndProcessId()
        {
            List<DelphiApplication> applications = IdentifyDelphiApplications();
            //
            // Find one atom by thread and one by process ID...
            //
            Assert.AreEqual(1, applications.Count);
            Assert.AreEqual("DelphiApp1", applications[0].ProcessName);
            Assert.AreEqual(2, applications[0].AtomCount);
            Assert.IsTrue(applications[0].Atoms.Contains("Delphi00000008"));
            Assert.IsTrue(applications[0].Atoms.Contains("ControlOfs00000508"));
            //
            // ...marking the other atom as unknown
            //
            List<string> unknownAtomNames = _delphiApplicationTracker.GetUnknownAtomNames();
            Assert.AreEqual(1, unknownAtomNames.Count);
            Assert.IsTrue(unknownAtomNames.Contains("SomethingElse"));

            _processNamesByThreadId = new Dictionary<int, string>
            {
                { 1200, "AnotherApp" },
            };
            _atoms = new List<AtomTableEntry>
            {
                new AtomTableEntry { Index = "AC", Name = "AnotherAppsAtom" },
            };

            applications = IdentifyDelphiApplications();
            //
            // Application is still tracked...
            //
            Assert.AreEqual(1, applications.Count);
            Assert.AreEqual("DelphiApp1", applications[0].ProcessName);
            Assert.AreEqual(2, applications[0].AtomCount);
            Assert.IsTrue(applications[0].Atoms.Contains("Delphi00000008"));
            Assert.IsTrue(applications[0].Atoms.Contains("ControlOfs00000508"));
            //
            // ...but atom "SomethingElse" was forgotten
            //
            unknownAtomNames = _delphiApplicationTracker.GetUnknownAtomNames();
            Assert.AreEqual(1, unknownAtomNames.Count);
            Assert.IsTrue(unknownAtomNames.Contains("AnotherAppsAtom"));

        }

        private List<DelphiApplication> IdentifyDelphiApplications()
        {
            _delphiApplicationTracker.IdentifyDelphiApplications(_atoms, _processNamesByProcessId, _processNamesByThreadId);
            return _delphiApplicationTracker.GetApplications();
        }
    }
}
