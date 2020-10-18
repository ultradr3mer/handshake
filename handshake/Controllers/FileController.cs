using Azure.Storage.Blobs;
using handshake.Contexts;
using handshake.Data;
using handshake.Entities;
using handshake.Interfaces;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="FileController"/> provides functionality to upload and download files.
  /// </summary>
  [Route("[controller]")]
  [Authorize]
  [ApiController]
  public class FileController : ControllerBase
  {
    #region Fields

    private const string UserContainerPrefix = "user-";

    private readonly IConfiguration configuration;
    private readonly UserDatabaseAccess userDatabaseAccess;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="FileController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    public FileController(IConfiguration configuration, IAuthService userService, UserDatabaseAccess userDatabaseAccess)
    {
      this.configuration = configuration;
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets an image.
    /// </summary>
    /// <param name="token">The access token of the image to get.</param>
    /// <param name="filename">The filename of the image to get.</param>
    /// <returns>The detailed post information.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("{token}/{filename}")]
    public async Task<IActionResult> Get(string token, string filename)
    {
      if (!long.TryParse(token, NumberStyles.HexNumber, null, out long actualToken))
      {
        throw new ArgumentException("Invalid Token.", nameof(token));
      }

      string contentType = this.GetContentTypeForExtension(Path.GetExtension(filename));

      string adminUsername = this.configuration["AdminUsername"];
      string adminPassword = this.configuration["AdminPassword"];

      await this.userService.Authenticate(adminUsername, adminPassword);
      using System.Data.SqlClient.SqlConnection connection = this.userService.Connection;

      using DatabaseContext context = new DatabaseContext(connection);
      string username = await (from t in context.FileAccessToken
                               join u in context.ShakeUser on t.User equals u.Id
                               where t.Token == actualToken
                               && t.Filename == filename
                               select u.Username).FirstOrDefaultAsync();

      if (string.IsNullOrEmpty(username))
      {
        throw new ArgumentException("Invalid Token.", nameof(token));
      }

      BlobContainerClient azureContainer = await this.GetAzureContainer(username);
      BlobClient blob = azureContainer.GetBlobClient(filename);
      Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> info = await blob.DownloadAsync();
      return this.File(info.Value.Content, contentType);
    }

    /// <summary>
    /// Saves a new file on the server.
    /// </summary>
    /// <param name="file">The actual file.</param>
    /// <returns>Ok, when upload succeded.</returns>
    [HttpPost("upload")]
    public async Task<FileTokenData> Post([FromForm] IFormFile file)
    {
      return await this.UploadInternal(file, overwrite: false);
    }

    /// <summary>
    /// Saves a new file on the server.
    /// </summary>
    /// <param name="file">The actual file.</param>
    /// <returns>Ok, when upload succeded.</returns>
    [HttpPost("uploadwithreplace")]
    public async Task<FileTokenData> PostWithReplace([FromForm] IFormFile file)
    {
      return await this.UploadInternal(file, overwrite: true);
    }

    private static async Task<long> CreateToken(string fileName, Guid userId, DatabaseContext context)
    {
      RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
      byte[] tokenBytes = new byte[8];
      provider.GetBytes(tokenBytes);
      long token = BitConverter.ToInt64(tokenBytes);
      FileAccessTokenEntity newToken = new FileAccessTokenEntity()
      {
        Token = token,
        Filename = fileName,
        User = userId
      };

      await context.FileAccessToken.AddAsync(newToken);
      await context.SaveChangesAsync();

      return newToken.Token;
    }

    private async Task<long> CreateTokenIfNotExists(string fileName)
    {
      using System.Data.SqlClient.SqlConnection connection = this.userService.Connection;
      GetData.ProfileGetData user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      using DatabaseContext context = new DatabaseContext(connection);
      long? existingToken = await this.TryGetToken(context, user.Id, fileName);
      if (existingToken != null)
      {
        return existingToken.Value;
      }

      long newToken = await CreateToken(fileName, user.Id, context);
      connection.Close();

      return newToken;
    }

    private async Task<BlobContainerClient> GetAzureContainer(string username)
    {
      string connectionString = this.configuration["AzureStorage_ConnectionString"];
      string containerName = UserContainerPrefix + username.ToLower();
      BlobContainerClient client = new BlobContainerClient(connectionString, containerName);
      await client.CreateIfNotExistsAsync();

      return client;
    }

    private string GetContentTypeForExtension(string extension)
    {
      switch (extension.ToLower())
      {
        case ".gif":
          return "image/gif";

        case ".png":
          return "image/png";

        case ".jpeg":
        case ".jpg":
        case ".jpe":
          return "image/jpeg";

        case ".zip":
          return "application/zip";

        default:
          throw new Exception("Invalid file extension.");
      }
    }

    private async Task<long?> TryGetToken(DatabaseContext context, Guid userId, string fileName)
    {
      long? token = await (from t in context.FileAccessToken
                           where t.Filename == fileName
                           && t.User == userId
                           select (long?)t.Token).FirstOrDefaultAsync();
      return token;
    }

    private async Task<FileTokenData> UploadInternal(IFormFile file, bool overwrite)
    {
      if(file.Length > 52428800)
      {
        throw new Exception("File exceeds 50MB.");
      }

      this.GetContentTypeForExtension(Path.GetExtension(file.FileName));

      long token = await this.CreateTokenIfNotExists(file.FileName);
      BlobContainerClient azureContainer = await this.GetAzureContainer(this.userService.Username);
      BlobClient blob = azureContainer.GetBlobClient(file.FileName);
      await blob.UploadAsync(file.OpenReadStream(), overwrite);

      return new FileTokenData()
      {
        Token = token.ToString("X"),
        FileName = file.FileName
      };
    }

    #endregion Methods
  }
}