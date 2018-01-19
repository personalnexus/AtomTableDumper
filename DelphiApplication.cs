using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace AtomTableDumper
{
    /// <summary>
    /// Represents not a single process, but rather all processes with the same name
    /// </summary>
    [Serializable]
    public class DelphiApplication
    {
        public DelphiApplication(string processName): this()
        {
            ProcessName = processName;
            FirstSeen = DateTime.Now;
        }

        public DelphiApplication()
        {
            _atoms = new HashSet<string>();
        }

        private HashSet<string> _atoms;

        public string ProcessName { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public int AtomCount { get { return _atoms.Count; } set { } }
        [XmlArrayItem(ElementName = "Atom")]
        public List<string> Atoms {  get { return _atoms.ToList(); } set { } }

        public void AddAtom(string atomName)
        {
            _atoms.Add(atomName);
            LastSeen = DateTime.Now;
        }
    }
}