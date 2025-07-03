using CheapInjector.Entity;

namespace CheapInjector
{
    /// <summary>
    /// DIコンテナクラス
    /// </summary>
    public static class Container
    {
        /// <summary>
        /// DI定義格納クラスリストを保持します。
        /// </summary>
        private static List<DefinitionsEntity> definitionsEntities;

        /// <summary>
        /// 読み込み対象ライブラリ情報格納クラスリストを保持します。
        /// </summary>
        private static List<ImplementLibraryEntity> implementLibraryEntities;

        /// <summary>
        /// 初期化済みか否かを保持します。
        /// </summary>
        private static bool IsInitialized = false;

        #region クラス初期化処理

        /// <summary>
        /// DI設定ファイルを読み込み、コンテナを初期化します。
        /// </summary>
        /// <param name="configurationFilePath">DI設定ファイルのパスを指定してください。</param>
        /// <exception cref="ArgumentNullException">DI設定ファイルのパスがnull、または空白の場合にスローします。</exception>
        /// <exception cref="FileNotFoundException">DI設定ファイルの存在が確認出来ない場合にスローします。</exception>
        /// <exception cref="Exception">DI設定ファイル読み込み失敗、アセンブリ読み込み失敗時にスローします。</exception>
        public static void Initialize(string configurationFilePath)
        {
            if (IsInitialized)
                return;

            if (string.IsNullOrWhiteSpace(configurationFilePath))
                throw new ArgumentNullException(nameof(configurationFilePath));

            if (!File.Exists(configurationFilePath))
                throw new FileNotFoundException(configurationFilePath);

            var readConfigurationResult = Configuration.GetConfiguration(configurationFilePath);
            if (!readConfigurationResult.DefinitionsEntities.Any() || !readConfigurationResult.ImplementLibraries.Any())
                throw new Exception($"Initialization failed. {configurationFilePath}");

            definitionsEntities = readConfigurationResult.DefinitionsEntities;
            implementLibraryEntities = readConfigurationResult.ImplementLibraries;

            implementLibraryEntities = Implement.LoadAssemblies(implementLibraryEntities);
            var loadFailed = implementLibraryEntities.Where(e => e.IsLoading == false).FirstOrDefault();
            if (loadFailed != null)
                throw new Exception($"Initialization failed. {configurationFilePath}");

            IsInitialized = true;
        }

        #endregion

        #region インスタンス作成処理

        /// <summary>
        /// 具象クラスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        public static T CreateInstance<T>()
        {
            return InstanceCreation<T>(string.Empty, null);
        }

        /// <summary>
        /// コンストラクタ引数を指定して、具象クラスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="args">コンストラクタに渡す引数を指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">引数がnullの場合にスローします。</exception>
        public static T CreateInstance<T>(object[] args)
        {
            if (args == null || !args.Any())
                throw new ArgumentNullException(nameof(args));

            return InstanceCreation<T>(string.Empty, args);
        }

        /// <summary>
        /// エイリアスを指定して、具象クラスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">エイリアスがnullの場合にスローします。</exception>
        public static T CreateInstance<T>(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            return InstanceCreation<T>(alias, null);
        }

        /// <summary>
        /// エイリアスとコンストラクタ引数を指定して、具象クラスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <param name="args">コンストラクタに渡す引数を指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">エイリアス、引数がnullの場合にスローします。</exception>
        public static T CreateInstance<T>(string alias, object[] args)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            if (args == null || !args.Any())
                throw new ArgumentNullException(nameof(args));

            return InstanceCreation<T>(alias, args);
        }

        /// <summary>
        /// 具象クラスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">エイリアスを指定してください。指定がない場合は、インターフェース名でインスタンスを作成します。</param>
        /// <param name="args">コンストラクタに渡す引数を指定してください。指定がない場合は、引数なしコンストラクタを実行します。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        private static T InstanceCreation<T>(string alias, object[] args)
        {
            var definitions = GetDefinisionsEntities<T>(alias);

            if (args == null || !args.Any())
                return Implement.GetInstance<T>(definitions[0]);
            else
                return Implement.GetInstance<T>(definitions[0], args);
        }

        /// <summary>
        /// 具象クラスのインスタンスをシングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        public static T CreateInstance_Singleton<T>()
        {
            return SingletonInstanceCreation<T>(string.Empty, null);
        }

        /// <summary>
        /// コンストラクタ引数を指定して、具象クラスのインスタンスをシングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="args">コンストラクタに渡す引数を指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">引数がnullまたはemptyの場合にスローします。</exception>
        public static T CreateInstance_Singleton<T>(object[] args)
        {
            if (args == null || !args.Any())
                throw new ArgumentNullException(nameof(args));

            return SingletonInstanceCreation<T>(string.Empty, args);
        }

        /// <summary>
        /// エイリアスを指定して、具象クラスのインスタンスをシングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">エイリアスがnullまたはemptyの場合にスローします。</exception>
        public static T CreateInstance_Singleton<T>(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            return SingletonInstanceCreation<T>(alias, null);
        }

        /// <summary>
        /// コンストラクタ引数とエイリアスを指定して、具象クラスのインスタンスをシングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <param name="args">コンストラクタに渡す引数を指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">エイリアス、引数がnullまたはemptyの場合にスローします。</exception>
        public static T CreateInstance_Singleton<T>(string alias, object[] args)
        {
            if (args == null || !args.Any())
                throw new ArgumentNullException(nameof(args));

            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            return SingletonInstanceCreation<T>(alias, args);
        }

        /// <summary>
        /// 具象クラスのインスタンスをシングルトンで作成します。
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">エイリアスを指定してください。指定がない場合は、インターフェース名でインスタンスを作成します。</param>
        /// <param name="args">コンストラクタに渡す引数を指定してください。指定がない場合は、引数なしコンストラクタを実行します。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        private static T SingletonInstanceCreation<T>(string alias, object[] args)
        {
            var definitions = GetDefinisionsEntities<T>(alias);

            if (args == null || !args.Any())
                return Implement.GetSingletonInstance<T>(definitions[0]);
            else
                return Implement.GetSingletonInstance<T>(definitions[0], args);
        }

        #endregion

        #region 作成済みインスタンス取得処理

        /// <summary>
        /// <para>シングルトンで作成されたインスタンスを取得します。</para>
        /// <para>インスタンスが未作成の場合、引数なしのコンストラクタ、シングルトンで作成します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        public static T GetInstance<T>()
        {
            return GetSingletonInstance<T>(string.Empty);
        }

        /// <summary>
        /// <para>エイリアスを指定して、シングルトンで作成されたインスタンスを取得します。</para>
        /// <para>インスタンスが未作成の場合、引数なしのコンストラクタ、シングルトンで作成します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルの登録されたエイリアスを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">エイリアスがnull、またはemptyの場合にスローします。</exception>
        public static T GetInstance<T>(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            return GetSingletonInstance<T>(alias);
        }

        /// <summary>
        /// <para>シングルトンで作成されたインスタンスを取得します。</para>
        /// <para>インスタンスが未作成の場合、引数なしのコンストラクタ、シングルトンで作成します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <returns>具象クラスのインスタンスを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        private static T GetSingletonInstance<T>(string alias)
        {
            var definitions = GetDefinisionsEntities<T>(alias);

            if (definitions[0].Instance != null)
                return (T)definitions[0].Instance;
            else
                return Implement.GetSingletonInstance<T>(definitions[0]);
        }

        #endregion

        #region インスタンス削除処理

        /// <summary>
        /// <para>シングルトンで作成されたインスタンスを削除します。</para>
        /// <para>指定されたインスタンスがIDisposableを継承する場合は、Disposeメソッドを実行して解放します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <returns>削除の結果を成功：true、失敗：falseで返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        public static bool DeleteSingletonInstance<T>()
        {
            return DeleteInstance<T>(string.Empty);
        }

        /// <summary>
        /// <para>シングルトンで作成されたインスタンスを削除します。</para>
        /// <para>指定されたインスタンスがIDisposableを継承する場合は、Disposeメソッドを実行して解放します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <returns>削除の結果を成功：true、失敗：falseで返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        /// <exception cref="ArgumentNullException">エイリアスがnull、またはemptyの場合にスローします。</exception>
        public static bool DeleteSingletonInstance<T>(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            return DeleteInstance<T>(alias);
        }

        /// <summary>
        /// <para>シングルトンで作成されたインスタンスを削除します。</para>
        /// <para>指定されたインスタンスがIDisposableを継承する場合は、Disposeメソッドを実行して解放します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">設定ファイルに登録されたエイリアスを指定してください。</param>
        /// <returns>削除の結果を成功：true、失敗：falseで返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        private static bool DeleteInstance<T>(string alias)
        {
            var definitions = GetDefinisionsEntities<T>(alias);

                        if (definitions[0].Instance != null && Implement.HasDispose(definitions[0].Instance))
            {
                if (!Implement.DisposeInstance(definitions[0].Instance))
                {
                    return false;
                }
            }

            definitions[0].Instance = null;
            return true;
        }

        #endregion

        #region インスタンス操作共通処理

        /// <summary>
        /// <para>パラメータ指定された型、またはエイリアス名からDI定義格納を取得します。</para>
        /// </summary>
        /// <typeparam name="T">インターフェースを指定してください。</typeparam>
        /// <param name="alias">エイリアス名を指定してください。</param>
        /// <returns>DI定義エンティティを返します。</returns>
        /// <exception cref="Exception">初期化が実行されていない場合、指定されたインターフェースが設定ファイルから取得できない場合にスローします。</exception>
        /// <exception cref="ArgumentException">指定された型パラメータがインターフェースではない場合にスローします。</exception>
        private static DefinitionsEntity[] GetDefinisionsEntities<T>(string alias)
        {
            if (!IsInitialized)
                throw new Exception("Not initialized.");

            var type = typeof(T);
            if (!type.IsInterface)
                throw new ArgumentException($"{type.Name} was not interface.");

            DefinitionsEntity[] definitions;
            if (string.IsNullOrWhiteSpace(alias))
                definitions = definitionsEntities.Where(e => e.InterfaceName == type.FullName).ToArray();
            else
                definitions = definitionsEntities.Where(e => e.Alias == alias).ToArray();

            if (definitions == null || definitions.Length != 1)
                throw new Exception($"{type.FullName} was not registed.");

            return definitions;
        }

        #endregion
    }
}