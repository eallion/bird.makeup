﻿using System;
using System.Net.Sockets;
using BirdsiteLive.Common.Interfaces;

namespace BirdsiteLive.Twitter.Models
{
    public class ExtractedTweet : SocialMediaPost
    {
        public long Id { get; set; }
        public long? InReplyToStatusId { get; set; }
        public string MessageContent { get; set; }
        public ExtractedMedia[] Media { get; set; }
        public DateTime CreatedAt { get; set; }
        public string InReplyToAccount { get; set; }
        public bool IsReply { get; set; }
        public bool IsThread { get; set; }
        public bool IsRetweet { get; set; }
        public string RetweetUrl { get; set; }
        public long RetweetId { get; set; }
        public TwitterUser OriginalAuthor { get; set; }
        public TwitterUser Author { get; set; }
    }
}