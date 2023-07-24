using TestInterfaceA;

namespace TestLibB
{
    internal class ClassA : IClassA
    {
        public string Message { get; set; }

        public ClassA()
        {
            Message = "TestLibBのClassAです。";
        }

        public string GetString()
        {
            return Message;
        }
    }
}
