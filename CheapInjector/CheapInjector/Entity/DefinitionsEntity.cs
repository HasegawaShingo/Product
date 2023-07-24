namespace CheapInjector.Entity
{
    /// <summary>
    /// DI定義格納クラス
    /// </summary>
    public class DefinitionsEntity
    {
        /// <summary>
        /// DI定義のエイリアスを格納します。
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// インターフェース名（FullName）を格納します。
        /// </summary>
        public string InterfaceName { get; set; }
        /// <summary>
        /// 具象クラスを実装したライブラリ名を格納します。
        /// </summary>
        public string LibararyName { get; set; }
        /// <summary>
        /// 具象クラスのクラス名（FullName）を格納します。
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 具象クラスのインスタンスを格納します。
        /// </summary>
        public object Instance { get; set; }
    }
}
