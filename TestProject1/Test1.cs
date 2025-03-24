using System.Reflection;
using FuzzingTests;

namespace TestProject1;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        foreach (string filePath in Directory.GetFiles("C:\\Users\\da180\\RiderProjects\\FuzzingTests\\TestProject1\\TestInput"))
        {
            string input = File.ReadAllText(filePath);
            Program.StartServer(
                [], 
                input
            );        
        }

    }
}