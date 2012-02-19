using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net;
using cf.Instrumentation;

namespace cf.Content.Images
{
    /// <summary>
    /// Saves images to Amazon S3 cloud storage
    /// </summary>
    public class AwsS3ImagePersister : AbstractImagePersister
    {
        AmazonS3Config S3Config = new AmazonS3Config()
        {
            ServiceURL = "s3.amazonaws.com",
            CommunicationProtocol = Amazon.S3.Model.Protocol.HTTP,
        };

        /// <summary>
        /// Saves image to images.climbfind.com
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        public override void SaveImage(Stream stream, string filePath, string key)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(Stgs.AWSAccessKey, Stgs.AWSSecretKey, S3Config))
                {
                    // simple object put
                    PutObjectRequest request = new PutObjectRequest();
                    request.WithBucketName("images.climbfind.com" + filePath);
                    request.WithInputStream(stream);
                    request.ContentType = "image/jpeg";
                    request.Key = key;
                    request.WithCannedACL(S3CannedACL.PublicRead);
                    client.PutObject(request);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }
        }

        /// <summary>
        /// Delete the specified image
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public override void DeleteImage(string filePath, string key)
        {
            DeleteObjectRequest request = new DeleteObjectRequest();
            
            request.WithBucketName("images.climbfind.com" + filePath)
                .WithKey(key);

            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(Stgs.AWSAccessKey, Stgs.AWSSecretKey, S3Config))
            {
                // simple object put
                using (DeleteObjectResponse response = client.DeleteObject(request))
                {
                    //-- Do a little bit of tracing
                    string headersString = string.Empty;
                    WebHeaderCollection headers = response.Headers;
                    foreach (string h in headers.Keys)
                    {
                        headersString += string.Format("Response Header: {0}, Value: {1}", h, headers.Get(h));
                    }
                    CfTrace.Information(TraceCode.DeletingImage, headersString);
                }
            }
        }
    }
}
