using System;
using System.Collections.Generic;
using System.Web;

namespace A2DWebRole1
{
    public class ForumPost : Microsoft.WindowsAzure.StorageClient.TableServiceEntity
    {
        public ForumPost()
        {
            PartitionKey = "a";
            RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }

        public DateTime CreatedOn { get; set; }

        public string UserName { get; set; }
        public string UserGUID { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }

        public string TopicName { get; set; }
        public string TopicGUID { get; set; }
        
        public string ForumName { get; set; }
        public string ForumGUID { get; set; }

        public string PostText { get; set; }
        public string PostGUID { get; set; }

    }
}