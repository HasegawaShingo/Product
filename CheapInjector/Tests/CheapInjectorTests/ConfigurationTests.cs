using NUnit.Framework;
using CheapInjector;
using System.IO;

namespace CheapInjectorTests
{
    /// <summary>
    /// DI設定ファイル読み込みクラスのテスト
    /// </summary>
    public class ConfigurationTests
    {
        private string baseConfigFilePath = $@"{Directory.GetCurrentDirectory()}\DefinitionFiles";

        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：3
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
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：失敗（FileNotFoundException発生）
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
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：0（ImplementLibrariesまたはImplementLibraryエレメントなし）
        /// 　DI定義格納クラスの要素数：3
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
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：0（Name属性なし）
        /// 　DI定義格納クラスの要素数：3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_Name()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：0（Name属性がEmpty）
        /// 　DI定義格納クラスの要素数：3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyName()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：0（Path属性なし）
        /// 　DI定義格納クラスの要素数：3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_Path()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：0（Path属性がEmpty）
        /// 　DI定義格納クラスの要素数：3
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyPath()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（Definitionsエレメントなし）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoElements_Definitions()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（Definitionエレメントなし）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoElement_Definition()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（Alias属性なし）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_Alias()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（Alias属性がEmpty）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyAlias()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（InterfaceName属性なし）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_InterfaceName()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（InterfaceName属性がEmpty）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyInterfaceName()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（LibraryName属性なし）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_LibraryName()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（LibraryName属性がEmpty）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyLibraryeName()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（FullName属性なし）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_FullName()
        {

        }

        /// <summary>
        /// DI設定ファイル読み込み試験。
        /// 期待する結果は以下のとおりです。
        /// 　処理：成功
        /// 　読み込み対象ライブラリ情報格納クラスの要素数：2
        /// 　DI定義格納クラスの要素数：0（FullName属性がEmpty）
        /// </summary>
        [Test]
        public void GetConfigurationTest_Success_NoAttribute_EmptyFullName()
        {

        }

    }
}