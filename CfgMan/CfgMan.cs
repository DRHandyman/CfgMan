namespace CfgMan
{
    internal class CfgManipulator
    {
        public enum Types
        {
            STRING = 0,
            SECTION = 1
        }

        List<String> lines = new();

        List<Tuple<String, String>> strs = new();

        List<Tuple<String, List<Tuple<String, String>>>> sections = new();

        public CfgManipulator() { }

        public CfgManipulator(String FilePath)
        {
            for (int i = 0; i < System.IO.File.ReadAllLines(FilePath).Count(); i++)
                lines.Add(System.IO.File.ReadAllLines(FilePath)[i]);

            FileRemoveExtraLines();

            ParseStrings();
            ParseSections();
        }


        public static bool FileExists(String FilePath)
        {
            return System.IO.File.Exists(FilePath);
        }

        void ParseStrings()
        {
            foreach (var i in lines)
            {
                if (!String.IsNullOrEmpty(i))
                {
                    String line = i;

                    if (LineIsSection(ref line))
                        break;
                    else if (LineIsString(ref line))
                    {
                        strs.RemoveAll(x => x.Item1 == GetStringName(ref line));
                        strs.Add(new(GetStringName(ref line), GetStringValue(i)));
                    }
                }
            }
        }

        String GetSectionName(String Section)
        {
            return Section.Substring(StringGetFirstCharacterIndex(Section, '[') + 1,
                StringGetLastCharacterIndex(Section, ']') - 1);
        }

        void ParseSections()
        {
            bool CanContinue = false;

            int CurrentSectionID = -1;

            foreach (var i in lines)
            {
                String line = i;

                if (LineIsSection(ref line) && !CanContinue)
                    CanContinue = true;

                if (CanContinue)
                {
                    if (LineIsSection(ref line))
                    {
                        CurrentSectionID++;

                        sections.Add(new(GetSectionName(line), new()));

                        continue;
                    }
                    if (CurrentSectionID > -1)
                    {
                        if (LineIsString(ref line))
                            sections[CurrentSectionID].Item2.Add(new(GetStringName(ref line), GetStringValue(line)));
                    }
                }
            }
        }

        void PrintAnError(String Message)
        {
            Console.WriteLine($"[CfgMan][Error]: {Message}");
        }

        public void Open(String FilePath)
        {
            new CfgManipulator(FilePath);
        }

        void FileRemoveExtraLines()
        {
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (!String.IsNullOrEmpty(lines[i]))
                    break;

                lines.RemoveAt(i);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (String.IsNullOrEmpty(lines[i]) &&
                    String.IsNullOrEmpty(lines[i + 1]) && i + 1 <= lines.Count)
                    lines.RemoveAt(i);
            }
        }

        int StringGetFirstCharacterIndex(String str, Char c)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c)
                    return i;
            }

            return -1;
        }

        int StringGetLastCharacterIndex(String str, Char c)
        {
            int output = -1;

            for (int i = 0; i < str.Length; i++)
                output = str[i] == c ? i : output;

            return output;
        }

        bool LineIsString(ref String line)
        {
            line = String.Concat(line.Where(c => !Char.IsWhiteSpace(c)));

            if (line.Count(c => (c == '=')) == 1 && line.Count(c => (c == '"')) >= 2 &&
                StringGetFirstCharacterIndex(line, '=') > 0 &&
                StringGetFirstCharacterIndex(line, '=') < StringGetFirstCharacterIndex(line, '"'))
                return true;
            else
                return false;
        }

        bool LineIsSection(ref String line)
        {
            line = String.Concat(line.Where(c => !Char.IsWhiteSpace(c)));

            if (!String.IsNullOrEmpty(line) && line[0] == '[' && line[line.Length - 1] == ']')
                return true;

            return false;
        }

        String GetStringName(ref string line)
        {
            line = String.Concat(line.Where(c => !Char.IsWhiteSpace(c)));

            return line.Substring(0, line.IndexOf('='));
        }

        String GetStringValue(String line)
        {
            string output = "";

            for (int i = StringGetFirstCharacterIndex(line, '"') + 1; i < StringGetLastCharacterIndex(line, '"'); i++)
                output += line[i];

            return output;
        }

        public bool Contains(String TypeName, Types Type)
        {
            if (Type == Types.STRING)
            {
                foreach (var i in strs)
                {
                    if (i.Item1 == TypeName)
                        return true;
                }
            }
            else if (Type == Types.SECTION)
            {
                foreach (var i in sections)
                {
                    if (i.Item1 == TypeName)
                        return true;
                }
            }

            return false;
        }

        public bool Contains(String SectionName, String StringName)
        {
            foreach (var i in sections)
            {
                if (i.Item1 == SectionName)
                {
                    foreach (var x in i.Item2)
                    {
                        if (x.Item1 == StringName)
                            return true;
                    }

                    break;
                }
            }

            return false;
        }

        public String Read(String StringName)
        {
            String output = "";

            foreach (var i in strs)
            {
                if (i.Item1 == StringName)
                    output = i.Item2;
            }

            return output;
        }

        public String Read(String SectionName, String StringName)
        {
            String output = "";

            foreach (var i in sections)
            {
                if (i.Item1 == SectionName)
                {
                    foreach (var x in i.Item2)
                    {
                        if (x.Item1 == StringName)
                            output = x.Item2;
                    }
                }
            }

            return output;
        }
    }
}
