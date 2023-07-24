using TestInterfaceB;

namespace TestLibB
{
    public class Class1 : IClassB, IDisposable
    {
        public string Message { get; set; }

        public Class1()
        {
            Message = "引数なしのコンストラクタでインスタンスを作成しました。";
        }

        public Class1(string message)
        {
            Message = message;
        }

        public void Dispose()
        {
        }

        public string GetString()
        {
            return Message;
        }
    }
}