using Azure.Storage.Blobs;
using handshake.Contexts;
using handshake.Data;
using handshake.Entities;
using handshake.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace handshake.Repositories
{
  /// <summary>
  /// The file repository.
  /// </summary>
  public class FileRepository
  {
    #region Fields

    private const string UserContainerPrefix = "user-";

    private IConfiguration configuration;
    private UserDatabaseAccess userDatabaseAccess;
    private IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="FileRepository"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    public FileRepository(IConfiguration configuration, IAuthService userService, UserDatabaseAccess userDatabaseAccess)
    {
      this.configuration = configuration;
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Creates the token for the file or retrives the existing one.
    /// </summary>
    /// <param name="fileName">The filename.</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The token.</returns>
    public async Task<FileAccessTokenEntity> CreateTokenIfNotExists(string fileName, SqlConnection connection)
    {
      GetData.ProfileGetData user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      using DatabaseContext context = new DatabaseContext(connection);
      FileAccessTokenEntity existingToken = await this.TryGetToken(context, user.Id, fileName);
      if (existingToken != null)
      {
        return existingToken;
      }

      FileAccessTokenEntity newToken = await CreateToken(fileName, user.Id, context);

      return newToken;
    }

    /// <summary>
    /// Retrives the corresponding content type for the file extension.
    /// </summary>
    /// <param name="extension">The file extension.</param>
    /// <returns>The content type.</returns>
    public string GetContentTypeForExtension(string extension)
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

    /// <summary>
    /// Gets the file coresponding to the token and filename.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="filename">The filename.</param>
    /// <returns>The container.</returns>
    public async Task<BlobContainerClient> GetFile(long token, string filename)
    {
      string adminUsername = this.configuration["AdminUsername"];
      string adminPassword = this.configuration["AdminPassword"];

      await this.userService.Authenticate(adminUsername, adminPassword);
      using SqlConnection connection = this.userService.Connection;

      using DatabaseContext context = new DatabaseContext(connection);
      string username = await (from t in context.FileAccessToken
                               join u in context.ShakeUser on t.User equals u.Id
                               where t.Token == token
                               && t.Filename == filename
                               select u.Username).FirstOrDefaultAsync();

      if (string.IsNullOrEmpty(username))
      {
        throw new ArgumentException("Invalid Token.", nameof(token));
      }

      return await this.GetAzureContainer(username);
    }

    /// <summary>
    /// The internal upload function.
    /// </summary>
    /// <param name="filename">The filename of the file to upload.</param>
    /// <param name="content">The content of the file to upload.</param>
    /// <param name="connection">The sql connection.</param>
    /// <param name="overwrite">True, if an existing file should be overwriten.</param>
    /// <returns>The token data of the uploaded file.</returns>
    public async Task<FileTokenData> UploadInternal(string filename, Stream content, SqlConnection connection, bool overwrite)
    {
      this.GetContentTypeForExtension(Path.GetExtension(filename));

      FileAccessTokenEntity tokenEntity = await this.CreateTokenIfNotExists(filename, connection);
      BlobContainerClient azureContainer = await this.GetAzureContainer(this.userService.Username);
      BlobClient blob = azureContainer.GetBlobClient(filename);
      await blob.UploadAsync(content, overwrite);

      return new FileTokenData(tokenEntity);
    }

    private static async Task<FileAccessTokenEntity> CreateToken(string fileName, Guid userId, DatabaseContext context)
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

    private async Task<FileAccessTokenEntity> TryGetToken(DatabaseContext context, Guid userId, string fileName)
    {
      FileAccessTokenEntity token = await (from t in context.FileAccessToken
                                           where t.Filename == fileName
                                           && t.User == userId
                                           select new FileAccessTokenEntity()
                                           {
                                             Id = t.Id,
                                             Filename = t.Filename,
                                             Token = t.Token,
                                           }).FirstOrDefaultAsync();
      return token;
    }

    #endregion Methods
  }
}