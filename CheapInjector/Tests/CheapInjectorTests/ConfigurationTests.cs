using NUnit.Framework;
using CheapInjector;
using System.IO;

namespace CheapInjectorTests
{
    /// <summary>
    /// DI�ݒ�t�@�C���ǂݍ��݃N���X�̃e�X�g
    /// </summary>
    public class ConfigurationTests
    {
        private string baseConfigFilePath = $@"{Directory.GetCurrentDirectory()}\DefinitionFiles";

        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success()
        {
            var configPath = Path.Combine(baseConfigFilePath, "InjectionSettings_GetConf_Success1.xml");
            var result = Configuration.GetConfiguration(configPath);

            if (result.ImplementLibraries.Count != 2)
                Assert.Fail();

            if (result.DefinitionsEntities.Count != 3)
                Assert.Fail();

            Assert.Pass();
        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F���s�iFileNotFoundException�����j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Fail_FileNotFound()
        {
            var configPath = Path.Combine(baseConfigFilePath, "InjectionSettings_NotFound.xml");

            try
            {
                var result = Configuration.GetConfiguration(configPath);
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F0�iImplementLibraries�܂���ImplementLibrary�G�������g�Ȃ��j
        /// �@DI��`�i�[�N���X�̗v�f���F3
        /// </summary>
        [TestCase("InjectionSettings_GetConf_NoElem_ImplLib.xml")]
        [TestCase("InjectionSettings_GetConf_NoElems_ImplLibs.xml")]
        public void GetConfigurationTest_Success_NoElements_ImplementLibraries(string filePath)
        {
            var configPath = Path.Combine(baseConfigFilePath, filePath);
            var result = Configuration.GetConfiguration(configPath);

            if (result.ImplementLibraries.Count != 0)
                Assert.Fail();

            if (result.DefinitionsEntities.Count != 3)
                Assert.Fail();

            Assert.Pass();
        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F0�iName�����Ȃ��j
        /// �@DI��`�i�[�N���X�̗v�f���F3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_Name()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F0�iName������Empty�j
        /// �@DI��`�i�[�N���X�̗v�f���F3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyName()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F0�iPath�����Ȃ��j
        /// �@DI��`�i�[�N���X�̗v�f���F3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_Path()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F0�iPath������Empty�j
        /// �@DI��`�i�[�N���X�̗v�f���F3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyPath()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iDefinitions�G�������g�Ȃ��j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoElements_Definitions()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iDefinition�G�������g�Ȃ��j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoElement_Definition()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iAlias�����Ȃ��j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_Alias()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iAlias������Empty�j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyAlias()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iInterfaceName�����Ȃ��j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_InterfaceName()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iInterfaceName������Empty�j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyInterfaceName()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iLibraryName�����Ȃ��j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_LibraryName()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iLibraryName������Empty�j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyLibraryeName()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iFullName�����Ȃ��j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_FullName()
        {

        }

        /// <summary>
        /// DI�ݒ�t�@�C���ǂݍ��ݎ����B
        /// ���҂��錋�ʂ͈ȉ��̂Ƃ���ł��B
        /// �@�����F����
        /// �@�ǂݍ��ݑΏۃ��C�u�������i�[�N���X�̗v�f���F2
        /// �@DI��`�i�[�N���X�̗v�f���F0�iFullName������Empty�j
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyFullName()
        {

        }

    }
}