using System.Text;
using todo.Logics.Entities;

namespace todo.Logics
{
    internal class TodoLogic
    {
        internal enum Status
        {
            NotStart,
            Working,
            Completed,
            Canceled
        }

        internal List<TodoItem> TodoItems;

        private readonly string _sourceFilePath;
        private readonly string _backupSourceFilePath;
        private readonly string _displaySettingFilePath;

        internal TodoLogic(string sourceFilePath, string displaySettingFilePath) 
        {
            _sourceFilePath = sourceFilePath ?? throw new ArgumentNullException(nameof(sourceFilePath));
            if (!File.Exists(sourceFilePath))
                CreateNewSourceFile();

            _displaySettingFilePath = displaySettingFilePath ?? throw new ArgumentNullException(nameof(displaySettingFilePath));
            if (!File.Exists(displaySettingFilePath))
                throw new FileNotFoundException(displaySettingFilePath);

            _backupSourceFilePath = GetBackupFilePath(sourceFilePath);
        }

        internal async Task<bool> GetTaskItemsAsync()
        {
            TodoItems = await XmlDataAccess.LoadDataAsync<List<TodoItem>>(_sourceFilePath);
            if (TodoItems.Any())
            {
                ReNumber();
                return true;
            }
            else
            {
                await CreateNewEntityAsync();
                return false;
            }
        }

        internal async Task<bool> SaveTaskItemsAsync()
        {
            for (int i = 0; i < TodoItems.Count; i++)
            {
                if (TodoItems[i].Status == (int)Status.Completed)
                    TodoItems.Remove(TodoItems[i]);
            }

            if (!TodoItems.Any())
            {
                ReNumber();
                File.Copy(_sourceFilePath, _backupSourceFilePath);
            }

            await XmlDataAccess.SaveDataAsync(TodoItems, _sourceFilePath);
            return true;
        }

        internal async Task<List<ListViewDisplayConfigEntity>> GetListDisplaySettings()
        {
            var settingEntities = await XmlDataAccess.LoadDataAsync<List<ListViewDisplayConfigEntity>>(_displaySettingFilePath);
            if (settingEntities == null || !settingEntities.Any())
                return new List<ListViewDisplayConfigEntity>();
            else
                return settingEntities;
        }

        internal async Task CreateNewEntityAsync()
        {
            await Task.Run(() =>
            {
                int maxID;
                if (TodoItems.Any())
                    maxID = TodoItems.Max(e => e.ID) + 1;
                else
                    maxID = 1;

                var entity = new TodoItem { ID = maxID, Status = 0 };
                TodoItems.Add(entity);
            });
        }

        internal async Task<bool> DeleteEntityAsync(int recordId)
        {
            bool isDeleted = false;
            await Task.Run(() =>
            {
                for(int i = 0; i < TodoItems.Count; i++)
                {
                    var entity = TodoItems[i];
                    if (entity.ID == recordId)
                    {
                        TodoItems.Remove(entity);
                        isDeleted = true;
                        break;
                    }
                }
                ReNumber();
            });

            return isDeleted;
        }

        internal void MoveUp(int recordID)
        {
            if (recordID < 0)
                return;

            var swapID = recordID - 1;
            SwapEntity(recordID, swapID);
        }

        internal void MoveDown(int recordID)
        {
            if (recordID > TodoItems.Count - 1)
                return;

            var swapID = recordID + 1;
            SwapEntity(recordID, swapID);
        }

        private void ReNumber()
        {
            var sortedEntities = TodoItems.OrderBy(e => e.ID).ToList();

            int ID = 1;
            foreach(var entity in sortedEntities)
            {
                entity.ID = ID;
                ID++;
            }
        }

        private void SwapEntity(int recordID, int swapID)
        {
            var targetEntity = TodoItems.Where(e => e.ID == recordID).FirstOrDefault();
            var swapEntity = TodoItems.Where(e => e.ID == swapID).FirstOrDefault();

            swapEntity.ID = recordID;
            targetEntity.ID = swapID;

            TodoItems = TodoItems.OrderBy(e => e.ID).ToList();
        }

        private static string GetBackupFilePath(string filePath)
        {
            var path = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            return $@"{path}\{fileName}_backup{Path.GetExtension(filePath)}";
        }

        private void CreateNewSourceFile()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>" + "\r\n");
            stringBuilder.Append(@"<ArrayOfTodoItem xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + "\r\n");
            stringBuilder.Append(@"</ArrayOfTodoItem>");

            using var streamWriter = new StreamWriter(_sourceFilePath, false, Encoding.UTF8);
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Flush();
        }
    }
}
