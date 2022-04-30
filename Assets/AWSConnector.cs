using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using System.Threading;

public class AWSConnector : MonoBehaviour
{
    const string bucketName = "your-bucket-name";
    private const string IDENTITY_POOL_ID = "your-pool-id";
    private const string SECRET_ACCESS_KEY = "your-secret-key";
    private RegionEndpoint cognito_region = RegionEndpoint.APNortheast1;
    private RegionEndpoint s3_region = RegionEndpoint.APNortheast1;
    private RegionEndpoint dynamo_region = RegionEndpoint.APNortheast1;
    private CognitoAWSCredentials credentials;
    private string ResultText;
    PutBucketResponse responseObject;
    string filePath;

    void Start()
    {
        filePath = Application.dataPath + @"\Scripts\File\WriteText1.txt";
        //ShowAllBuckets();
        //CreateFile();
        // PostObjects();
        GetObjects();
    }

   private async void CreateS3Bucket()
    {
        credentials = new CognitoAWSCredentials(
          IDENTITY_POOL_ID,
          cognito_region
          );
        var s3Client = new AmazonS3Client(credentials, RegionEndpoint.APNortheast1);

        var putBucketRequest = new PutBucketRequest
        {
            BucketName = bucketName,
            UseClientRegion = true,
        };

        Task<PutBucketResponse> responseTask = s3Client.PutBucketAsync(putBucketRequest);

        try
        {
            PutBucketResponse responseObject = await responseTask;
            Debug.Log($"Created a bucket '{bucketName}'");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async void ShowAllBuckets()
    {
        credentials = new CognitoAWSCredentials(
         IDENTITY_POOL_ID,
         cognito_region
         );
        var s3Client = new AmazonS3Client(IDENTITY_POOL_ID,SECRET_ACCESS_KEY,s3_region);
        ResultText = "Fetching all the Buckets";
        var allBuckets = await s3Client.ListBucketsAsync();
        foreach(var buckets in allBuckets.Buckets)
        {
            Debug.Log(buckets.BucketName);
        }
    }

    private void CreateFile()
    {
        if (!File.Exists(filePath))
        {
            using (File.Create(filePath))
            {
            }
        }

        string myString1 = "かきくけこ\nさしすせそ\n";
        File.AppendAllText(filePath, myString1);

        string[] myStringArray = { "あいうえお", "かきくけこ", "さしすせそ" };
        File.WriteAllLines(filePath, myStringArray);

        string myString2 = "あいうえお\nかきくけこ\nさしすせそ\nたちつてと\nなにぬねの";
        File.WriteAllText(filePath, myString2);

    }

    private async void PostObjects()
    {
        credentials = new CognitoAWSCredentials(
       IDENTITY_POOL_ID,
       cognito_region
       );
        var s3Client = new AmazonS3Client(IDENTITY_POOL_ID, SECRET_ACCESS_KEY, s3_region);
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = filePath,
                InputStream = stream,
                CannedACL = S3CannedACL.Private
            };
            await s3Client.PutObjectAsync(request);

        }
      
    }

    private async void GetObjects()
    {
        credentials = new CognitoAWSCredentials(
      IDENTITY_POOL_ID,
      cognito_region
      );
        var s3Client = new AmazonS3Client(IDENTITY_POOL_ID, SECRET_ACCESS_KEY, s3_region);
        GetObjectResponse result = await s3Client.GetObjectAsync(bucketName, filePath);
        string data = null;
        using(StreamReader reader = new StreamReader(result.ResponseStream))
        {
            data = reader.ReadToEnd();
        }
        Debug.Log(data);

    }
  
}
