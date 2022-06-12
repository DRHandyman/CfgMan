namespace test
{
    class Program
    {
        static void Main()
        {
            CfgMan.CfgMan m_CfgMan = new("test.cfg");

            Console.WriteLine(m_CfgMan.read("string"));
        }
    }
}