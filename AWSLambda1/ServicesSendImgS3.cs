using Amazon.S3;
using Amazon.S3.Model;

namespace AWSLambda1
{
    public class ServicesSendImgS3
    {
        readonly string BucketName = "bukect-name";
        string CodeImg = Guid.NewGuid().ToString();

        public async Task<string> LoadImgBase64(IAmazonS3 S3, LoadImg loadImg)
        {
            string url = String.Empty;
            string base64String = loadImg.Image.Contains(',') ? loadImg.Image.Split(",")[1] : loadImg.Image;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);

                using (S3)
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = BucketName,
                        ContentType = "image/png",
                        CannedACL = S3CannedACL.PublicRead,
                        Key = CodeImg,
                    };

                    using (var ms = new MemoryStream(bytes))
                    {
                        request.InputStream = ms;
                        PutObjectResponse? response = await S3.PutObjectAsync(request);
                    }

                    // Share the uploaded file and get a download URL
                    string uploadedFileUrl = S3.GetPreSignedURL(new GetPreSignedUrlRequest()
                    {
                        BucketName = BucketName,
                        Key = CodeImg,
                        Expires = DateTime.Now.AddMinutes(30)
                    });

                    url = uploadedFileUrl.Split("?")[0];
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Funtion LoadImgBase64 Fail {ex}");
            }

            return await Task.FromResult(url);
        }
    }
}
