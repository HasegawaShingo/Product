namespace CheapInjector.Entity
{
    /// <summary>
    /// 読み込み対象ライブラリ情報格納クラス
    /// </summary>
    public class ImplementLibraryEntity
    {
        /// <summary>
        /// ライブラリ名称を保持します。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ライブラリのパスを保持します。
        /// </summary>
        public string LibraryPath { get; set; }
        /// <summary>
        /// ライブラリが読み込み済みか否かを保持します。
        /// </summary>
        public bool IsLoading { get; set; }
    }
}
