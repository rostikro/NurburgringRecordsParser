using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurburgringRecordsParser.GoogleDrive
{
    internal class API
    {
        static API _instance;

        string _jsonCredentials = "{\"installed\":{\"client_id\":\"256493343603-3h17k26380uh1lncfq596pgkfgd16mac.apps.googleusercontent.com\",\"project_id\":\"diamond-sheets\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"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\",\"redirect_uris\":[\"http://localhost\"]}}";
        string _tokensPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "NurburgringRecords\\tokens");
        DriveService _service;

        API()
        {
            try
            {
                UserCredential credential;

                using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(_jsonCredentials)))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new List<string> { DriveService.Scope.Drive },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(_tokensPath, true)).Result;
                }

                _service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Nurburgring Records Parser",
                });
            }
            catch (AggregateException e)
            {
                Debug.WriteLine("CredentialNotFound");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static API GetInstance()
        {
            if (_instance == null)
            {
                _instance = new API();
            }

            return _instance;
        }

        public async Task<string> UploadFile(MemoryStream fileStream, string fileName, string contentType)
        {
            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName
                };

                FilesResource.CreateMediaUpload request;
                request = _service.Files.Create(fileMetadata, fileStream, contentType);
                request.Fields = "id";
                await request.UploadAsync();

                var file = request.ResponseBody;

                Debug.WriteLine(file.Id);
                return file.Id;
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine("File not found");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
    }
}
