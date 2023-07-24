namespace todo.Logics.Entities
{
    public class TodoItem
    {
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public string SubSystemName { get; set; }
        public string ProcessName { get; set; }
        public string ParentTaskName { get; set; }
        public string TaskName { get; set; }
        public int EstimatedTime { get; set; }
        public int ActualTime { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
    }
}
