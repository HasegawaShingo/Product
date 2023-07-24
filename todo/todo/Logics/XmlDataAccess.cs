using System.Text;
using System.Xml.Serialization;

namespace todo.Logics
{
    internal static class XmlDataAccess
    {

        internal static async Task SaveDataAsync<T>(T items, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var streamWriter = new StreamWriter(filePath, false, new UTF8Encoding(false));
            serializer.Serialize(streamWriter, items);
            await streamWriter.FlushAsync();
        }

        internal static async Task<T> LoadDataAsync<T>(string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var streamReader = new StreamReader(filePath, new UTF8Encoding(false));
            return await Task.Run(() => (T)serializer.Deserialize(streamReader));
        }

    }
}
