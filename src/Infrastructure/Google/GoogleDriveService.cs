using Galliard.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using File = Google.Apis.Drive.v3.Data.File;

namespace Microsoft.Extensions.DependencyInjection.Google;

public class GoogleDriveService(ILogger<GoogleDriveService> logger, IConfiguration configuration,
    IFileService fileService) : IGoogleDriveService
{

    private const string APP_NAME = "Galliard";
    
    public async Task<string> UploadNovelization(string novelizationFilePath)
    {
        GoogleCredential credential = await GetGoogleCredential();
        var driveService = LoadDriveService(credential);
        var docsService = LoadDocsService(credential);
        
        var documentId = await CreateGoogleDriveDocument(driveService);
        AddNovelization(docsService, novelizationFilePath, documentId);
        
        await SetPermissionsToOwner(driveService, documentId);
        
        return documentId;
    }

    private async Task SetPermissionsToOwner(DriveService driveService, string documentId)
    {
        var ownerMail = configuration.GetValue<string>("GoogleDrive:OwnerMail");
        
        var permission = new Permission
        {
            Type = "user",
            Role = "writer",
            EmailAddress = ownerMail!
        };
        await driveService.Permissions.Create(permission, documentId).ExecuteAsync();

        logger.LogInformation($"Permission granted to {ownerMail} to edit document {documentId}");
    }

    private async void AddNovelization(DocsService docsService, string novelizationFilePath, string documentId)
    {
        logger.LogInformation($"Adding novelization to document {documentId}...");
        var novelizationText = await fileService.ReadTextFile(novelizationFilePath);
        var requests = new List<Request>
        {
            new()
            {
                InsertText = new InsertTextRequest
                {
                    Location = new Location { Index = 1 },
                    Text = novelizationText
                }
            }
        };
        
        var batchUpdateRequest = new BatchUpdateDocumentRequest { Requests = requests };
        await docsService.Documents.BatchUpdate(batchUpdateRequest, documentId).ExecuteAsync();
        logger.LogInformation($"Document {documentId} updated");
    }

    private DocsService LoadDocsService(GoogleCredential credential)
    {
        logger.LogInformation("Creating Google Docs service...");
        var docsService = new DocsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = APP_NAME
        });
        
        return docsService;
    }

    private DriveService LoadDriveService(GoogleCredential credential)
    {
        logger.LogInformation("Creating Google Drive service...");
        var driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = APP_NAME
        });
        
        return driveService;
    }

    private async Task<GoogleCredential> GetGoogleCredential()
    {
        var credentialsFile = configuration.GetValue<string>("GoogleDrive:AuthJson");
     
        logger.LogInformation("Loading Google credentials...");
        await using var stream = new FileStream(credentialsFile!, FileMode.Open, FileAccess.Read);
        GoogleCredential credential = GoogleCredential.FromStream(stream)
            .CreateScoped(DocsService.Scope.Documents, DriveService.Scope.Drive);

        return credential;
    }

    private async Task<string> CreateGoogleDriveDocument(DriveService driveService)
    {
        var folderId = configuration.GetValue<string>("GoogleDrive:FolderId");
        
        logger.LogInformation("Creating Google Drive document...");
        var fileMetadata = new File
        {
            Name  = $"Transcripción partida {DateTime.Now:yyyyMMddHHmmss}",
            MimeType = "application/vnd.google-apps.document",
            Parents = new List<string> { folderId! }
        };
        
        var createRequest = driveService.Files.Create(fileMetadata);
        createRequest.Fields = "id";
        var createdFile = await createRequest.ExecuteAsync();
        var documentId = createdFile.Id;
        logger.LogInformation($"Google Drive document created: {documentId}");
        
        return documentId;
    }
}
