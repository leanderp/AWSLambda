using Amazon.Lambda.Core;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda1;

public class Function
{
    IAmazonS3 S3Client { get; set; }
    private readonly ServicesSendImgS3 ServiceSendImg;

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
        ServiceSendImg = new ServicesSendImgS3();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
        ServiceSendImg = new ServicesSendImgS3();
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="imput"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string?> FunctionHandler(LoadImg imput, ILambdaContext context)
    {
        if(imput.Image is null)
        {
            return null;
        }

        try
        {
            var url = await ServiceSendImg.LoadImgBase64(S3Client, imput);

            return url;
        }
        catch (Exception e)
        {
            context.Logger.LogInformation(e.Message);
            context.Logger.LogInformation(e.StackTrace);
            return null;
        }
    }
}