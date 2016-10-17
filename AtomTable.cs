using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace AtomTableDumper
{
    [Serializable]
    public class AtomTable
    {
        public AtomTable()
        {
            RegisteredWindowMessages = new List<AtomTableEntry>();
            GlobalAtoms = new List<AtomTableEntry>();
        }

        public int RegisteredWindowMessageCount { get; set; }

        public int GlobalAtomCount { get; set; }

        [XmlArrayItem(ElementName = "RegisteredWindowMessage")]
        public List<AtomTableEntry> RegisteredWindowMessages { get; set; }

        [XmlArrayItem(ElementName = "GlobalAtom")]
        public List<AtomTableEntry> GlobalAtoms { get; set; }

        private static void GetAtomTableEntries(IList<AtomTableEntry> atomTableEntries, Func<int, StringBuilder, int, int> getAtomTableEntry)
        {
            atomTableEntries.Clear();
            var buffer = new StringBuilder(1024);

            for (int index = 0xC000; index <= 0xFFFF; index++)
            {
                int bufferLength = getAtomTableEntry(index, buffer, buffer.Capacity);
                if (bufferLength > 0)
                {
                    var atomTableEntry = new AtomTableEntry
                    {
                        Index = index.ToString("x").ToUpper(),
                        Name = buffer.ToString(0, bufferLength)
                    };
                    atomTableEntries.Add(atomTableEntry);
                }
            }
        }

        public void Load()
        {
            GetAtomTableEntries(RegisteredWindowMessages, (index, buffer, bufferCapacity) => NativeMethods.GetClipboardFormatName((uint)index, buffer, bufferCapacity));
            GetAtomTableEntries(GlobalAtoms, (index, buffer, bufferCapacity) => (int)NativeMethods.GlobalGetAtomName((ushort)index, buffer, bufferCapacity));
            //
            // These two fields are provided as a convenience in the output file, so the counts are easy to see when viewing the file in a text editor
            //
            RegisteredWindowMessageCount = RegisteredWindowMessages.Count;
            GlobalAtomCount = GlobalAtoms.Count;
        }
    }
}
