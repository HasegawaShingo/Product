using TestInterfaceA;

namespace TestLibA
{
    public class ClassA : IClassA
    {
        private string _message;
        public string Message { get { return _message; } set { _message = value; } }

        public ClassA()
        {
            _message = "引数なしコンストラクタでインスタンスが作成されました。";
        }

        public ClassA(string message)
        {
            _message = message;
        }

        public string GetString()
        {
            return _message;
        }
    }
}