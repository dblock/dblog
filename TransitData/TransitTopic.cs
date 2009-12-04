using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitTopic : TransitObject
    {
        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        private string mType;

        public string Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }

        public TransitTopic()
        {

        }

        public TransitTopic(DBlog.Data.Topic o)
            : base(o.Id)
        {
            Name = o.Name;
        }

        public Topic GetTopic(ISession session)
        {
            Topic topic = (Id != 0) ? (Topic)session.Load(typeof(Topic), Id) : new Topic();
            topic.Name = Name;
            return topic;
        }

        public static void MergeTo(
            ISession session,
            Post currentPost, 
            IEnumerable<TransitTopic> newPostTopics,
            out List<PostTopic> toBeCreated,
            out List<PostTopic> toBeDeleted)
        {
            List<TransitTopic> workingPostTopics = new List<TransitTopic>();
            if (newPostTopics != null) workingPostTopics.AddRange(newPostTopics);
            toBeDeleted = new List<PostTopic>();
            toBeCreated = new List<PostTopic>();

            // find all topics that don't need to change and all those that need to be removed
            if (currentPost.PostTopics != null)
            {
                foreach (PostTopic existingTopic in currentPost.PostTopics)
                {
                    bool found = false;
                    if (workingPostTopics != null)
                    {
                        foreach (TransitTopic workingTopic in workingPostTopics)
                        {
                            if (workingTopic.Id == existingTopic.Topic.Id)
                            {
                                workingPostTopics.Remove(workingTopic);
                                found = true;
                                break;
                            }
                        }
                    }

                    // not found, needs to be removed
                    if (! found)
                    {
                        toBeDeleted.Add(existingTopic);
                    }
                }
            }

            // remaining working topics should be created
            if (workingPostTopics != null)
            {
                foreach (TransitTopic workingTopic in workingPostTopics)
                {
                    PostTopic postTopic = new PostTopic();
                    postTopic.Post = currentPost;
                    postTopic.Topic = workingTopic.GetTopic(session);
                    toBeCreated.Add(postTopic);
                }
            }
        }
    }
}
