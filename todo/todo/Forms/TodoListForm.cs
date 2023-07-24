using System.Collections.Specialized;
using System.Configuration;
using todo.Forms;
using todo.Logics;
using todo.Logics.Entities;

namespace todo
{
    public partial class TodoListForm : Form
    {
        private Point mousePoint;
        private readonly TodoLogic todoLogic;
        private Dictionary<string, int> comboboxSourceDictionary;
        private Dictionary<string, Image> statusImageDictionary;

        public TodoListForm()
        {
            InitializeComponent();

            LabelTitle.Text = $"{LabelTitle.Text}  ({DateTime.Now:yyyy/MM/dd})";

            var sourceFilePath = ConfigurationManager.AppSettings["TodoSourceFilePath"].ToString();
            var listDisplaySettingFilePath = ConfigurationManager.AppSettings["ListDisplaySettingFilePath"].ToString();
            todoLogic = new TodoLogic(sourceFilePath, listDisplaySettingFilePath);
        }

        private void SplitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                mousePoint = new Point(e.X, e.Y);
        }

        private void SplitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Left += e.X - mousePoint.X;
                Top += e.Y - mousePoint.Y;
            }
        }

        private async void ButtonExit_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                var saveResult = await todoLogic.SaveTaskItemsAsync();
                if (!saveResult)
                {
                    MessageBox.Show("データ保存に失敗しました。ごめんなさい。", "Todo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("データ保存に失敗しました。ごめんなさい。", "Todo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.WriteFatal($"{nameof(ButtonExit_ClickAsync)}", $"exception : {ex.Message}\r\nstack trace : {ex.StackTrace}");
            }

            Close();
        }

        private async void TodoListForm_LoadAsync(object sender, EventArgs e)
        {
            try
            {
                var iconList = (NameValueCollection)ConfigurationManager.GetSection("IconList");
                await LoadStatusIconAsync(iconList);
                await CreateStatusComboboxSourceAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteFatal($"{nameof(TodoListForm_LoadAsync)}", ex.Message);
                MessageBox.Show("プログラムの初期化に失敗しました。\r\nプログラムを終了します。", "Todo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            try
            {
                if (!await todoLogic.GetTaskItemsAsync())
                    Logger.WriteDebug($"{nameof(TodoListForm_LoadAsync)}", $"create new datasource file.");

                TodoList.DataSource = todoLogic.TodoItems;
                await SetListColumnAsync();
                await CreateImageColumnAsync();
                TodoList.AutoResizeColumns();
                TodoList.AutoResizeRows();
                TodoList.ClearSelection();
            }
            catch (Exception ex)
            {
                Logger.WriteFatal($"{nameof(TodoListForm_LoadAsync)}", ex.Message);
                MessageBox.Show("タスクリストの読み込みに失敗しました。\r\nプログラムを終了します。", "Todo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private async Task LoadStatusIconAsync(NameValueCollection iconList)
        {
            await Task.Run(() =>
            {
                statusImageDictionary = new Dictionary<string, Image>();
                foreach (string item in iconList)
                {
                    var imagePath = iconList.GetValues(item).FirstOrDefault();
                    if (!File.Exists(imagePath))
                        throw new FileNotFoundException(imagePath);
                    statusImageDictionary.Add(item, Image.FromFile(imagePath));
                }
            });
        }

        private async Task CreateStatusComboboxSourceAsync()
        {
            await Task.Run(() =>
            {
                comboboxSourceDictionary = new Dictionary<string, int>
                {
                    { TodoLogic.Status.NotStart.ToString(), (int)TodoLogic.Status.NotStart },
                    { TodoLogic.Status.Working.ToString(), (int)TodoLogic.Status.Working },
                    { TodoLogic.Status.Completed.ToString(), (int)TodoLogic.Status.Completed },
                    { TodoLogic.Status.Canceled.ToString(), (int)TodoLogic.Status.Canceled },
                };
            });
        }

        private async Task SetListColumnAsync()
        {
            var settings = await todoLogic.GetListDisplaySettings();
            if (!settings.Any())
                return;

            foreach (DataGridViewColumn column in TodoList.Columns)
            {
                var setting = settings.Where(item => item.Name == column.Name).FirstOrDefault();
                if (setting == null)
                    continue;

                column.HeaderText = setting.DisplayName;
                column.Visible = Convert.ToBoolean(setting.Visible);
            }

            var comboboxColumnSetting = settings.Where(item => item.Type == "StatusCombobox").FirstOrDefault();
            if (comboboxColumnSetting == null)
                return;

            var comboColumn = TodoList.Columns[comboboxColumnSetting.Name];
            if (comboColumn == null)
                return;

            await CreateStatusComboboxColumnAsync(comboboxColumnSetting, comboColumn);
        }

        private async Task CreateStatusComboboxColumnAsync(ListViewDisplayConfigEntity displayConfigEntity, DataGridViewColumn column)
        {
            await Task.Run(() =>
            {
                var comboboxColumn = new DataGridViewComboBoxColumn
                {
                    AutoComplete = false,
                    DataPropertyName = column.DataPropertyName,
                    DataSource = comboboxSourceDictionary.ToList(),
                    DisplayMember = "key",
                    ValueMember = "value",
                    FlatStyle = FlatStyle.Flat,
                    HeaderText = displayConfigEntity.DisplayName
                };

                void ReplaceColumn()
                {
                    TodoList.Columns.Insert(column.Index, comboboxColumn);
                    var name = column.Name;
                    TodoList.Columns.Remove(name);
                    comboboxColumn.Name = name;
                }

                Invoke(ReplaceColumn);
            });
        }

        private async Task CreateImageColumnAsync()
        {
            await Task.Run(() =>
            {
                if (TodoList.Columns[0].Name == "StatusImage")
                    return;

                var imageColumn = new DataGridViewImageColumn
                {
                    Name = "StatusImage",
                    HeaderText = "状態",
                    Image = statusImageDictionary[TodoLogic.Status.NotStart.ToString()],
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };

                imageColumn.DefaultCellStyle.BackColor = Color.Black;

                void InsertColumn()
                {
                    TodoList.Columns.Insert(0, imageColumn);
                    TodoList.CellFormatting += TodoList_CellFormatting;
                }

                Invoke(InsertColumn);
            });
        }

        private void TodoList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var statusColumnName = "Status";
            var statusImageColumnName = "StatusImage";
            var datagridView = sender as DataGridView;

            if (!datagridView.Columns.Contains("StatusImage"))
                return;

            if (datagridView.Columns[e.ColumnIndex].Name == statusColumnName && e.RowIndex >= 0)
            {
                var value = (int)datagridView[statusColumnName, e.RowIndex].Value;
                switch (value)
                {
                    case (int)TodoLogic.Status.NotStart:
                        datagridView[statusImageColumnName, e.RowIndex].Value = statusImageDictionary[TodoLogic.Status.NotStart.ToString()];
                        e.FormattingApplied = true;
                        break;
                    case (int)TodoLogic.Status.Working:
                        datagridView[statusImageColumnName, e.RowIndex].Value = statusImageDictionary[TodoLogic.Status.Working.ToString()];
                        e.FormattingApplied = true;
                        break;
                    case (int)TodoLogic.Status.Completed:
                        datagridView[statusImageColumnName, e.RowIndex].Value = statusImageDictionary[TodoLogic.Status.Completed.ToString()];
                        e.FormattingApplied = true;
                        break;
                    case (int)TodoLogic.Status.Canceled:
                        datagridView[statusImageColumnName, e.RowIndex].Value = statusImageDictionary[TodoLogic.Status.Canceled.ToString()];
                        e.FormattingApplied = true;
                        break;
                }
            }
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TodoList.CurrentRow.Index == -1 || TodoList.CurrentRow.Index == 0)
                return;

            var currentRowNumber = TodoList.CurrentRow.Index - 1;
            var currentRow = TodoList.CurrentRow.DataBoundItem as TodoItem;
            todoLogic.MoveUp(currentRow.ID);

            RefreshList(currentRowNumber);
            Logger.WriteDebug($"{nameof(UpToolStripMenuItem_Click)}", $"move row [up] : row index {currentRowNumber + 1} to {currentRowNumber}");
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TodoList.CurrentRow.Index == -1 || TodoList.CurrentRow.Index == TodoList.RowCount - 1)
                return;

            var currentRowNumber = TodoList.CurrentRow.Index + 1;
            var currentRow = TodoList.CurrentRow.DataBoundItem as TodoItem;
            todoLogic.MoveDown(currentRow.ID);

            RefreshList(currentRowNumber);
            Logger.WriteDebug($"{nameof(DownToolStripMenuItem_Click)}", $"move row [down] : row index {currentRowNumber - 1} to {currentRowNumber}");
        }

        private void RefreshList(int selectedRowIndex)
        {
            TodoList.DataSource = todoLogic.TodoItems;
            TodoList.Rows[selectedRowIndex].Selected = true;
        }

        private async void CreateItemToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            await todoLogic.CreateNewEntityAsync();
            await UpdateTodoListDatasource();
            TodoList.Rows[^1].Selected = true;

            Logger.WriteDebug($"{nameof(CreateItemToolStripMenuItem_ClickAsync)}", $"create row : new row index was {TodoList.Rows.Count + 1}");
        }

        private async void DeleteItemToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            var currentRowIndex = TodoList.CurrentRow.Index;
            var currentRow = TodoList.CurrentRow.DataBoundItem as TodoItem;
            var deleteResult = await todoLogic.DeleteEntityAsync(currentRow.ID);

            if (deleteResult)
            {
                await UpdateTodoListDatasource();
                TodoList.ClearSelection();

                Logger.WriteDebug($"{nameof(DeleteItemToolStripMenuItem_ClickAsync)}", $"delete row : delete row index was {currentRowIndex}");
            }
            else
            {
                MessageBox.Show("行の削除に失敗しました。", "Todo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private async Task UpdateTodoListDatasource()
        {
            TodoList.DataSource = null;
            TodoList.Columns.RemoveAt(0);
            TodoList.DataSource = todoLogic.TodoItems;

            await SetListColumnAsync();
            await CreateImageColumnAsync();
            TodoList.AutoResizeColumns();
            TodoList.AutoResizeRows();
        }

        private void ContextMenuStrip_Opened(object sender, EventArgs e)
        {
            if (TodoList.DataSource == null || TodoList.CurrentRow == null)
            {
                UpToolStripMenuItem.Enabled = false;
                DownToolStripMenuItem.Enabled = false;
                DeleteItemDToolStripMenuItem.Enabled = false;
                return;
            }

            var rowCount = TodoList.RowCount;

            if (rowCount == 0)
            {
                UpToolStripMenuItem.Enabled = false;
                DownToolStripMenuItem.Enabled = false;
                DeleteItemDToolStripMenuItem.Enabled = false;
            }
            else if (rowCount == 1)
            {
                UpToolStripMenuItem.Enabled = false;
                DownToolStripMenuItem.Enabled = false;
                DeleteItemDToolStripMenuItem.Enabled = true;
            }
            else
            {
                UpToolStripMenuItem.Enabled = true;
                DownToolStripMenuItem.Enabled = true;
                DeleteItemDToolStripMenuItem.Enabled = true;
            }
        }

        private void TodoList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("データ設定時にエラーが発生しました。アプリケーションを終了します。", "Todo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Logger.WriteFatal($"{nameof(TodoList_DataError)}", $"exception : {e.Exception.Message} \r\nstack trace : {e.Exception.StackTrace}");
            Close();
        }

        private void ShowLicense_Click(object sender, EventArgs e)
        {
            var licenseViewer = new LicenseView();
            licenseViewer.ShowDialog(this);
        }
    }
}