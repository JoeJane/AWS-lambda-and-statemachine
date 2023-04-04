using Amazon.DynamoDBv2.DataModel;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection
{
    [DynamoDBTable("ImageDetails")]

    public class ImageDetails
    {
        [DynamoDBHashKey]
        public String S3ObjectURL { get; set; }
        public List<Tag> Tags { get; set; }

        public ImageDetails() { }
        public ImageDetails(string s3ObjectURL, List<Tag> tags)
        {
            S3ObjectURL = s3ObjectURL;
            Tags = tags;
        }
    }

    public class Tags
    {
        public string Label { get; set; }
        public string ConfidenceLevel { get; set; }
        public Tags() { }

        public Tags(string label, string confidenceLevel)
        {
            Label = label;
            ConfidenceLevel = confidenceLevel;
        }
    }
}
