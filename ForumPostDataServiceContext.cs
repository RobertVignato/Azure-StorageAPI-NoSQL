using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace A2DWebRole1
{
    public class ForumPostDataServiceContext : TableServiceContext
    {
        public ForumPostDataServiceContext(string baseAddress, StorageCredentials credentials) : base(baseAddress, credentials)
        {
        }


        public IQueryable<ForumPost> ForumPosts
        {
            get
            {
                return this.CreateQuery<ForumPost>("ForumPosts");
            }
        }

        public void AddForumPost(
            DateTime createdOn
            , string userName
            , string userGUID
            , string fullName
            , string topicName
            , string topicGUID
            , string forumName
            , string forumGUID
            , string postText
            , string postGUID
            )
        {
            this.AddObject("ForumPosts", new ForumPost { 
                CreatedOn = createdOn
                , UserName = userName
                , UserGUID = userGUID
                , FullName = fullName
                , TopicName = topicName
                , TopicGUID = topicGUID
                , ForumName = forumName
                , ForumGUID = forumGUID
                , PostText = postText
                , PostGUID = postGUID
            });
            this.SaveChanges();
        }
        
    }
}