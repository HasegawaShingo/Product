using System.Reflection;
using CheapInjector.Entity;

namespace CheapInjector
{
    /// <summary>
    /// ライブラリ読み込み、具象クラスインスタンス作成クラス
    /// </summary>
    internal static class Implement
    {
        /// <summary>
        /// ライブラリを読み込み、引数のエンティティリスト内プロパティを読み込み済みとして更新します。
        /// </summary>
        /// <param name="implementLibraryEntities">読み込み対象ライブラリ情報格納クラスリストを指定してください。</param>
        /// <returns>更新した読み込み対象ライブラリ情報格納クラスリストを返します。</returns>
        internal static List<ImplementLibraryEntity> LoadAssemblies(List<ImplementLibraryEntity> implementLibraryEntities)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var loadedAssemblyNames = loadedAssemblies.Select(e => e.GetName());

            foreach(var library in implementLibraryEntities)
            {
                foreach(var assemblyName in loadedAssemblyNames)
                {
                    if (library.Name == assemblyName.Name)
                        library.IsLoading = true;
                }
            }

            var needToReadAssemblies = implementLibraryEntities.Where(e => e.IsLoading == false);
            foreach(var item in needToReadAssemblies)
            {
                var assembly = Assembly.LoadFrom(item.LibraryPath);
                if (assembly.GetName().Name == item.Name)
                    item.IsLoading = true;
            }

            return implementLibraryEntities;
        }

        /// <summary>
        /// 具象クラスのインスタンスを引数なしで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="definitionsEntity">インスタンス作成対象のDI定義格納クラスを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        internal static T GetInstance<T>(DefinitionsEntity definitionsEntity)
        {
            return GetInstance<T>(definitionsEntity, null);
        }

        /// <summary>
        /// 具象クラスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="definitionsEntity">インスタンス作成対象のDI定義格納クラスを指定してください。</param>
        /// <param name="args">引数ありでインスタンスを作成する場合は指定してください。引数なしの場合はnullを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">インスタンス作成対象の具象クラスが、指定されたインターフェースを実装していない場合にスローします。</exception>
        internal static T GetInstance<T>(DefinitionsEntity definitionsEntity, object[] args)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.Where(e => e.GetName().Name == definitionsEntity.LibararyName).FirstOrDefault();
            var implementType = assembly.GetType(definitionsEntity.FullName);

            var interfaceName = implementType.GetInterfaces().Where(e => e.FullName == definitionsEntity.InterfaceName).FirstOrDefault();
            if (interfaceName == null)
                throw new Exception($"{definitionsEntity.FullName} was not implemented target interface.");

            if (args == null || !args.Any())
                return (T)Activator.CreateInstance(implementType);
            else
                return (T)Activator.CreateInstance(implementType, args);
        }

        /// <summary>
        /// 具象クラスのインスタンスを引数なし、シングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="definitionsEntity">インスタンス作成対象のDI定義格納クラスを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        internal static T GetSingletonInstance<T>(DefinitionsEntity definitionsEntity)
        {
            return GetSingletonInstance<T>(definitionsEntity, null);
        }

        /// <summary>
        /// 具象クラスのインスタンスをシングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="definitionsEntity">インスタンス作成対象のDI定義格納クラスを指定してください。</param>
        /// <param name="args">引数ありでインスタンスを作成する場合は指定してください。引数なしの場合はnullを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        internal static T GetSingletonInstance<T>(DefinitionsEntity definitionsEntity, object[] args)
        {
            if (definitionsEntity.Instance != null)
                return (T)definitionsEntity.Instance;

            var instance = GetInstance<T>(definitionsEntity, args);
            definitionsEntity.Instance = instance;
            return instance;
        }

        /// <summary>
        /// インスタンスがDisposeメソッドを実装しているかを取得します。
        /// </summary>
        /// <param name="instance">取得対象のインスタンスを指定してください。</param>
        /// <returns>実装あり：true、実装なし：falseを返します。</returns>
        /// <exception cref="ArgumentNullException">インスタンスがnullの場合にスローします。</exception>
        internal static bool HasDispose(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var iDisposable = instance.GetType().GetInterfaces().Where(e => e.Name == nameof(IDisposable)).FirstOrDefault();
            if (iDisposable == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// インスタンスのDisposeメソッドを実行します。
        /// </summary>
        /// <param name="instance">実行対象のインスタンスを指定してください。</param>
        /// <returns>Dispose実行成功：true、Dispose実行失敗：falseを返します。</returns>
        /// <exception cref="ArgumentNullException">インスタンスがnullの場合にスローします。</exception>
        internal static bool DisposeInstance(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var invokeResult = instance.GetType().GetMethod("Dispose").Invoke(instance, null);
            if (invokeResult == null)
                return true;
            else
                return false;
        }
    }
}
