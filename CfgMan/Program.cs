using CfgMan;

namespace test
{
    class Program
    {
        static void Main()
        {
            CfgManipulator m_CfgManipulator;

            if (CfgManipulator.FileExists("test.cfg"))
            {
                m_CfgManipulator = new("test.cfg");

                if (m_CfgManipulator.Contains("section", CfgManipulator.Types.SECTION))
                {
                    if (m_CfgManipulator.Contains("section", "string"))
                        Console.WriteLine(m_CfgManipulator.Read("section", "string"));
                }
            }
        }
    }
}