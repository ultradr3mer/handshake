using Azure.Storage.Blobs;
using handshake.Data;
using handshake.Interfaces;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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

    private readonly FileRepository fileRepository;
    private readonly UserDatabaseAccess userDatabaseAccess;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    /// <param name="fileRepository">The file repository.</param>
    public FileController(IAuthService userService, UserDatabaseAccess userDatabaseAccess, FileRepository fileRepository)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
      this.fileRepository = fileRepository;
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

      string contentType = this.fileRepository.GetContentTypeForExtension(Path.GetExtension(filename));

      BlobContainerClient azureContainer = await this.fileRepository.GetFile(actualToken, filename);
      BlobClient blob = azureContainer.GetBlobClient(filename);
      Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> info = await blob.DownloadAsync();
      return this.File(info.Value.Content, contentType);
    }

    /// <summary>
    /// Saves a new file on the server.
    /// </summary>
    /// <param name="file">The actual file.</param>
    /// <returns>Ok, when upload succeded.</returns>
    [HttpPost("Upload")]
    public async Task<FileTokenData> Post(IFormFile file)
    {
      using SqlConnection connection = this.userService.Connection;
      var result = await this.fileRepository.UploadInternal(file.FileName, file.OpenReadStream(), connection, overwrite: false);
      connection.Close();

      return result;
    }

    /// <summary>
    /// Saves a new file on the server.
    /// </summary>
    /// <param name="file">The actual file.</param>
    /// <returns>Ok, when upload succeded.</returns>
    [HttpPost("UploadWithReplace")]
    public async Task<FileTokenData> PostWithReplace(IFormFile file)
    {
      using SqlConnection connection = this.userService.Connection;
      var result = await this.fileRepository.UploadInternal(file.FileName, file.OpenReadStream(), connection, overwrite: true);
      connection.Close();

      return result;
    }

    #endregion Methods

  }
}