namespace Snoop.Client.Services.QueryModels
{
    public class TableQueryModel
    {
        public string TableName { get; set; }
        public string SchemaName { get; set; }

        public string GetQualifiedName() => $"[{SchemaName}].[{TableName}]";
    }
}