namespace CfgMan
{
    internal class CfgMan
    {
        List<String> lines = new();

        List<Tuple<String, String>> strs = new();

        List<Tuple<String, List<String>>> sections;

        public CfgMan() { }

        public CfgMan(String FilePath)
        {
            ScanFileExtension(FilePath);

            for (int i = 0; i < System.IO.File.ReadAllLines(FilePath).Count(); i++)
                lines.Add(System.IO.File.ReadAllLines(FilePath)[i]);

            FileRemoveExtraLines();

            foreach (var s in lines)
            {
                String line = s;

                if (LineIsString(ref line))
                    strs.Add(new(GetStringName(ref line), GetStringValue(line)));
            }
        }

        void PrintAnError(String Message)
        {
            Console.WriteLine($"[CfgMan][Error]: {Message}");
        }

        void ScanFileExtension(String FilePath)
        {
            bool result = false;

            string[] AllowedExtensions = { ".cfg", ".conf", ".config" };

            foreach (var i in AllowedExtensions)
            {
                if (System.IO.Path.GetExtension(FilePath) == i)
                    result = true;
            }

            if (!result)
            {
                PrintAnError($"The \"{FilePath}\" file extension is not allowed. Acceptable formats: {String.Join(", ", AllowedExtensions)}.");

                Environment.Exit(1);
            }
        }

        public void Open(String FilePath)
        {
            new CfgMan(FilePath);
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

            foreach (var i in lines)
                Console.WriteLine(i);
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

        public String read(String StringName)
        {
            String output = "";

            bool found = false;

            foreach (var i in strs)
            {
                if (i.Item1 == StringName)
                {
                    output = i.Item2;

                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine($"[CfgMan][Error]: Could not find the string called \"{StringName}\"...");

                Environment.Exit(1);
            }

            return output;
        }
    }
}
