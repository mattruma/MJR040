using Microsoft.Azure.Cosmos.Table;

namespace FunctionApp1
{
    public class ToDoEntityDataStoreOptions
    {
        public CloudTableClient PrimaryCloudTableClient { get; set; }
        public CloudTableClient SecondaryCloudTableClient { get; set; }
    }
}
