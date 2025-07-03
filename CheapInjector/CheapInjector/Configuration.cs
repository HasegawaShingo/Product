using System.Xml.Linq;
using System.Xml.XPath;
using CheapInjector.Entity;

namespace CheapInjector
{
    /// <summary>
    /// DI設定ファイル読み込みクラス
    /// </summary>
    internal static class Configuration
    {
        /// <summary>
        /// DI設定ファイルを読み込み、読み込み対象ライブラリ情報格納クラス、DI定義格納クラスのリストを作成します。
        /// </summary>
        /// <param name="configurationFilePath">読み込み対象のDI設定ファイルパスを指定してください。</param>
        /// <returns>読み込み対象ライブラリ情報格納クラスリスト、DI定義格納クラスリストを返します。</returns>
        internal static (List<ImplementLibraryEntity> ImplementLibraries, List<DefinitionsEntity> DefinitionsEntities) GetConfiguration(string configurationFilePath)
        {
            var sourceXml = XDocument.Load(configurationFilePath);
            var implementLibraryElements = sourceXml.XPathSelectElements("root/ImplementLibraries/ImplementLibrary");
            var implementLibraries = GetImplementLibraries(implementLibraryElements);
            var definitionsElements = sourceXml.XPathSelectElements("root/Definitions/Definition");
            var definitionsEntities = GetDefinitions(definitionsElements);
            return (implementLibraries, definitionsEntities);
        }

        /// <summary>
        /// DI設定ファイルから読み込み対象ライブラリ情報を読み込みます。
        /// </summary>
        /// <param name="elements">読み込み対象ライブラリ情報のエレメントを指定してください。</param>
        /// <returns>読み込み対象ライブラリ情報格納クラスリストを返します。</returns>
        private static List<ImplementLibraryEntity> GetImplementLibraries(IEnumerable<XElement> elements)
        {
            var result = new List<ImplementLibraryEntity>();
            if (elements == null || !elements.Any()) return result;

            foreach (var element in elements)
            {
                var nameAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("name", StringComparison.OrdinalIgnoreCase));
                var pathAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("path", StringComparison.OrdinalIgnoreCase));

                if (nameAttribute == null || pathAttribute == null || string.IsNullOrWhiteSpace(nameAttribute.Value) || string.IsNullOrWhiteSpace(pathAttribute.Value))
                    continue;

                var entity = new ImplementLibraryEntity { Name = nameAttribute.Value, LibraryPath = pathAttribute.Value, IsLoading = false };
                result.Add(entity);
            }
            return result;
        }

        /// <summary>
        /// DI設定ファイルからDI定義情報を読み込みます。
        /// </summary>
        /// <param name="elements">DI定義情報のエレメントを指定してください。</param>
        /// <returns>DI定義格納クラスリストを返します。</returns>
        /// <exception cref="ArgumentException">DI定義情報のエイリアスが重複している場合にスローします。</exception>
        private static List<DefinitionsEntity> GetDefinitions(IEnumerable<XElement> elements)
        {
            var result = new List<DefinitionsEntity>();
            if (elements == null || !elements.Any()) return result;

            foreach (var element in elements)
            {
                var aliasAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("alias", StringComparison.OrdinalIgnoreCase));
                var interfaceNameAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("interfacename", StringComparison.OrdinalIgnoreCase));
                var libraryNameAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("libraryname", StringComparison.OrdinalIgnoreCase));
                var fullNameAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("fullname", StringComparison.OrdinalIgnoreCase));

                if (interfaceNameAttribute == null || libraryNameAttribute == null || fullNameAttribute == null ||
                    string.IsNullOrWhiteSpace(interfaceNameAttribute.Value) || string.IsNullOrWhiteSpace(libraryNameAttribute.Value) || string.IsNullOrWhiteSpace(fullNameAttribute.Value))
                    continue;

                var entity = new DefinitionsEntity
                {
                    Alias = aliasAttribute?.Value ?? string.Empty,
                    InterfaceName = interfaceNameAttribute.Value,
                    LibararyName = libraryNameAttribute.Value,
                    FullName = fullNameAttribute.Value,
                    Instance = null
                };
                result.Add(entity);
            }

            var aliasesWithValues = result.Where(e => !string.IsNullOrWhiteSpace(e.Alias)).Select(e => e.Alias);
            if (aliasesWithValues.Count() != aliasesWithValues.Distinct().Count())
                throw new ArgumentException("Alias was duplicated.");

            return result;
        }
    }
}
