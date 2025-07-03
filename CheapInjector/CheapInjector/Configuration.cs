using System.Xml.Linq;
using System.Xml.XPath;
using CheapInjector.Entity;

namespace CheapInjector
{
    /// <summary>
    /// DI�ݒ�t�@�C���ǂݍ��݃N���X
    /// </summary>
    internal static class Configuration
    {
        /// <summary>
        /// DI�ݒ�t�@�C����ǂݍ��݁A�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�ADI��`�i�[�N���X�̃��X�g���쐬���܂��B
        /// </summary>
        /// <param name="configurationFilePath">�ǂݍ��ݑΏۂ�DI�ݒ�t�@�C���p�X���w�肵�Ă��������B</param>
        /// <returns>�ǂݍ��ݑΏۃ��C�u�������i�[�N���X���X�g�ADI��`�i�[�N���X���X�g��Ԃ��܂��B</returns>
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
        /// DI�ݒ�t�@�C������ǂݍ��ݑΏۃ��C�u��������ǂݍ��݂܂��B
        /// </summary>
        /// <param name="elements">�ǂݍ��ݑΏۃ��C�u�������̃G�������g���w�肵�Ă��������B</param>
        /// <returns>�ǂݍ��ݑΏۃ��C�u�������i�[�N���X���X�g��Ԃ��܂��B</returns>
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
        /// DI�ݒ�t�@�C������DI��`����ǂݍ��݂܂��B
        /// </summary>
        /// <param name="elements">DI��`���̃G�������g���w�肵�Ă��������B</param>
        /// <returns>DI��`�i�[�N���X���X�g��Ԃ��܂��B</returns>
        /// <exception cref="ArgumentException">DI��`���̃G�C���A�X���d�����Ă���ꍇ�ɃX���[���܂��B</exception>
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
