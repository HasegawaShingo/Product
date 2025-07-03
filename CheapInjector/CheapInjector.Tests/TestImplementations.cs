
namespace CheapInjector.Tests.TestImplementations
{
    public interface IService
    {
        string Greet();
    }

    public class ServiceImplementation : IService
    {
        public string Greet() => "Hello from ServiceImplementation";
    }

    public class AnotherServiceImplementation : IService
    {
        public string Greet() => "Hello from AnotherServiceImplementation";
    }

        public interface IServiceWithConstructor
    {
        string Greet();
    }

    public class ServiceWithConstructor : IServiceWithConstructor
    {
        private readonly string _message;
        public ServiceWithConstructor(string message)
        {
            _message = message;
        }
        public string Greet() => _message;
    }

    public interface IDisposableService : IDisposable
    {
        bool IsDisposed { get; }
    }

    public class DisposableService : IDisposableService
    {
        public bool IsDisposed { get; private set; } = false;
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
