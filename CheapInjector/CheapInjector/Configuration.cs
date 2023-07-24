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

            if (elements == null || !elements.Any())
                return result;

            foreach (var element in elements)
            {
                var attributes = element.Attributes();
                var nameAttribute = attributes.Where(e => e.Name.ToString().ToLower() == "name").FirstOrDefault();
                var pathAttribute = attributes.Where(e => e.Name.ToString().ToLower() == "path").FirstOrDefault();

                if (nameAttribute == null || pathAttribute == null)
                    continue;

                if (string.IsNullOrWhiteSpace(nameAttribute.Value) || string.IsNullOrWhiteSpace(pathAttribute.Value))
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

            if (elements == null || !elements.Any())
                return result;

            var aliases = elements.Attributes().Where(e => e.Name.ToString().ToLower() == "alias").Select(e => e.Value);
            var aliasCount = aliases.Count();
            var distinctedAliasCount = aliases.Distinct().Count();
            if (aliasCount != distinctedAliasCount)
                throw new ArgumentException("Alias was duplicated.");

            foreach (var element in elements)
            {
                var attributes = element.Attributes();
                var aliasAttribute = attributes.Where(e => e.Name.ToString().ToLower() == "alias").FirstOrDefault();
                var interfaceNameAttribute = attributes.Where(e => e.Name.ToString().ToLower() == "interfacename").FirstOrDefault();
                var libraryNameAttribute = attributes.Where(e => e.Name.ToString().ToLower() == "libraryname").FirstOrDefault();
                var fullNameAttrinute = attributes.Where(e => e.Name.ToString().ToLower() == "fullname").FirstOrDefault();

                if (aliasAttribute == null || interfaceNameAttribute == null || libraryNameAttribute == null || fullNameAttrinute == null)
                    continue;

                if (string.IsNullOrWhiteSpace(aliasAttribute.Value) || string.IsNullOrWhiteSpace(interfaceNameAttribute.Value) || string.IsNullOrWhiteSpace(libraryNameAttribute.Value) || string.IsNullOrWhiteSpace(fullNameAttrinute.Value))
                    continue;

                var entity = new DefinitionsEntity
                {
                    Alias = aliasAttribute.Value,
                    InterfaceName = interfaceNameAttribute.Value,
                    LibararyName = libraryNameAttribute.Value,
                    FullName = fullNameAttrinute.Value,
                    Instance = null
                };
                result.Add(entity);
            }

            return result;
        }

    }
}
