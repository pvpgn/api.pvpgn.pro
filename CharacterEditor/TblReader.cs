using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CharacterEditor
{
    /// <summary>
    /// Reader of Diablo 2 *.tbl files
    /// </summary>
    public class TblReader
    {
        Dictionary<string, Dictionary<string, string>> tables;

        public TblReader(string file) : this(new string[] { file })  { }

        public TblReader(string[] files)
        {
            tables = new Dictionary<string, Dictionary<string, string>>();
            // sort tables by descending, to search from the end
            //  for example, if files = { "string.tbl", "expansionstring.tbl", "patchstring.tbl" }
            //  then it will search starting from "patchstring.tbl"
            foreach (var f in files.OrderByDescending(t => t))
            {
                tables[f] = readFile(f);
            }
        }

        /// <summary>
        /// Find value by key in all preloaded tables
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string FindString(string key)
        {
            foreach (var t in tables)
            {
                if (t.Value.ContainsKey(key))
                    return t.Value[key];
            }
            return string.Empty;
        }

        private Dictionary<string, string> readFile(string fileName)
        {
            var header = new TblHeader();
            var hashNodes = new List<TblHashNode>();
            var nodes = new Dictionary<string, string>();

            using (var fs = Resources.Instance.OpenResource(fileName))
            using (var br = new BinaryReader(fs))
            {
                // read header
                header.CRC = br.ReadInt16();
                header.NodesNumber = br.ReadInt16();
                header.HashTableSize = br.ReadInt32();
                header.Version = br.ReadByte();
                header.DataStartOffset = br.ReadInt32();
                header.HashMaxTries = br.ReadInt32();
                header.FileSize = br.ReadInt32();

                // skip indices
                fs.Seek(header.NodesNumber * sizeof(short), SeekOrigin.Current); // 2 - sizeof short

                // read hashes
                for (var i = 0; i < header.HashTableSize; i++)
                {
                    var hNode = new TblHashNode();

                    hNode.Active = br.ReadByte();
                    hNode.Index = br.ReadInt16();
                    hNode.HashValue = br.ReadInt32();
                    hNode.StringKeyOffset = br.ReadInt32();
                    hNode.StringValOffset = br.ReadInt32();
                    hNode.StringValLength = br.ReadInt16();
                    hashNodes.Add(hNode);
                }
                // read data
                for (var i = 0; i < header.HashTableSize; i++)
                {
                    // entry is not used, i.e. it's deleted
                    if (hashNodes[i].Active == 0)
                        continue;

                    fs.Position = hashNodes[i].StringKeyOffset;

                    var key = Encoding.ASCII.GetString(br.ReadBytes(hashNodes[i].StringValOffset - hashNodes[i].StringKeyOffset)).TrimEnd('\0');
                    var val = Encoding.ASCII.GetString(br.ReadBytes(hashNodes[i].StringValLength)).TrimEnd('\0');

                    // ignore collisions
                    // FIXME: may be replace?
                    nodes.TryAdd(key, val);
                }
            }
            return nodes;
        }

        public struct TblHeader
        {
            public short CRC;               // +0x00 - CRC value for string table
            public short NodesNumber;       // +0x02 - size of Indices array
            public int HashTableSize;       // +0x04 - size of TblHashNode array
            public byte Version;            // +0x08 - file version, either 0 or 1, doesn't matter
            public int DataStartOffset;     // +0x09 - string table start offset
            public int HashMaxTries;        // +0x0D - max number of collisions for string key search based on its hash value
            public int FileSize;            // +0x11 - size of the file
        }

        public struct TblHashNode
        {
            public byte Active;             // +0x00 - shows if the entry is used. if 0, then it has been "deleted" from the table
            public short Index;             // +0x01 - index in Indices array
            public int HashValue;           // +0x03 - hash value of the current string key
            public int StringKeyOffset;     // +0x07 - offset of the current string key
            public int StringValOffset;     // +0x0B - offset of the current offset
            public short StringValLength;   // +0x0F - length of the current string value

        }
        public struct TblDataNode
        {
            public string Key;              // current string key
            public string Val;              // current string value
        }
    }
}
