using Microsoft.Azure.Cosmos.Table;

namespace ClassLibrary2
{
    public class EntityDataStoreOptions
    {
        public CloudTableClient PrimaryCloudTableClient { get; set; }
        public CloudTableClient SecondaryCloudTableClient { get; set; }

        public EntityDataStoreOptions()
        {
        }

        public EntityDataStoreOptions(
            CloudTableClient primaryCloudTableClient,
            CloudTableClient secondaryCloudTableClient)
        {
            this.PrimaryCloudTableClient = primaryCloudTableClient;
            this.SecondaryCloudTableClient = secondaryCloudTableClient;
        }
    }
}
