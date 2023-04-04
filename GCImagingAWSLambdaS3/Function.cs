using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GCImagingAWSLambdaS3;

public class Function
{
    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var s3Event = evnt.Records?[0].S3;
        if (s3Event == null)
        {
            return null;
        }

        try
        {
            var rs = await this.S3Client.GetObjectMetadataAsync(
                s3Event.Bucket.Name,
                s3Event.Object.Key);

            if (rs.Headers.ContentType.StartsWith("image/"))
            {
                using (GetObjectResponse response = await S3Client.GetObjectAsync(
                    s3Event.Bucket.Name,
                    s3Event.Object.Key))
                {
                    using (Stream responseStream = response.ResponseStream)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            using (var memstream = new MemoryStream())
                            {
                                var buffer = new byte[512];
                                var bytesRead = default(int);
                                while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    memstream.Write(buffer, 0, bytesRead);
                                // Perform image manipulation 
                                var transformedImage = GcImagingOperations.GetConvertedImage(memstream.ToArray());
                                byte[] image = Convert.FromBase64String(transformedImage);

                                PutObjectRequest putRequest = new PutObjectRequest()
                                {
                                    BucketName = "bucket4lab4thumbnailjane",
                                    Key = $"thumbnail/grayscale-{s3Event.Object.Key}",
                                };
                                using (var mem = new MemoryStream(image))
                                {
                                    putRequest.InputStream = mem;
                                    await S3Client.PutObjectAsync(putRequest);
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Thumbnail Uploaded to S3 Bucket Successfully");
            return rs.Headers.ContentType;
        }
        catch (Exception e)
        {
            throw;
            Console.WriteLine("Thumbnail Created Failed");

        }
    }
}