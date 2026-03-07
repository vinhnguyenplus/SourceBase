// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Renci.SshNet;

namespace Admin.NET.Core
{
    /// <summary>
    /// SSH/Sftp tools
    /// </summary>
    public class SSHHelper : IDisposable
    {
        private readonly SftpClient _sftp;

        public SSHHelper(string host, int port, string user, string password)
        {
            _sftp = new SftpClient(host, port, user, password);
        }

        /// <summary>
        /// connect
        /// </summary>
        private void Connect()
        {
            if (!_sftp.IsConnected)
                _sftp.Connect();
        }

        /// <summary>
        /// Does a file with the same name exist?
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <returns></returns>
        public bool Exists(string ftpFileName)
        {
            Connect();

            return _sftp.Exists(ftpFileName);
        }

        /// <summary>
        /// Delete files
        /// </summary>
        /// <param name="ftpFileName"></param>
        public void DeleteFile(string ftpFileName)
        {
            Connect();

            _sftp.DeleteFile(ftpFileName);
        }

        /// <summary>
        /// Download to the specified directory
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <param name="localFileName"></param>
        public void DownloadFile(string ftpFileName, string localFileName)
        {
            Connect();

            using (Stream fileStream = File.OpenWrite(localFileName))
            {
                _sftp.DownloadFile(ftpFileName, fileStream);
            }
        }

        /// <summary>
        /// Read bytes
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <returns></returns>
        public byte[] ReadAllBytes(string ftpFileName)
        {
            Connect();

            return _sftp.ReadAllBytes(ftpFileName);
        }

        /// <summary>
        /// Read stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream OpenRead(string path)
        {
            return _sftp.Open(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Continue downloading
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <param name="localFileName"></param>
        public void DownloadFileWithResume(string ftpFileName, string localFileName)
        {
            DownloadFile(ftpFileName, localFileName);
        }

        /// <summary>
        /// Rename
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public void RenameFile(string oldPath, string newPath)
        {
            _sftp.RenameFile(oldPath, newPath);
        }

        /// <summary>
        /// Files in specified directory
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<string> GetFileList(string folder, IEnumerable<string> filters)
        {
            Connect();

            var files = new List<string>();
            var sftpFiles = _sftp.ListDirectory(folder);
            foreach (var file in sftpFiles)
            {
                if (file.IsRegularFile && filters.Any(f => file.Name.EndsWith(f)))
                    files.Add(file.Name);
            }
            return files;
        }

        /// <summary>
        /// Upload files in the specified directory
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="ftpFileName"></param>
        public void UploadFile(string localFileName, string ftpFileName)
        {
            Connect();

            var dir = Path.GetDirectoryName(ftpFileName);
            CreateDir(_sftp, dir);
            using (var fileStream = new FileStream(localFileName, FileMode.Open))
            {
                _sftp.UploadFile(fileStream, ftpFileName);
            }
        }

        /// <summary>
        /// Upload bytes
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="ftpFileName"></param>
        public void UploadFile(byte[] bs, string ftpFileName)
        {
            Connect();

            var dir = Path.GetDirectoryName(ftpFileName);
            CreateDir(_sftp, dir);
            _sftp.WriteAllBytes(ftpFileName, bs);
        }

        /// <summary>
        /// Upload stream
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="ftpFileName"></param>
        public void UploadFile(Stream fileStream, string ftpFileName)
        {
            Connect();

            var dir = Path.GetDirectoryName(ftpFileName);
            CreateDir(_sftp, dir);
            _sftp.UploadFile(fileStream, ftpFileName);
            fileStream.Dispose();
        }

        /// <summary>
        /// Create directory
        /// </summary>
        /// <param name="sftp"></param>
        /// <param name="dir"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void CreateDir(SftpClient sftp, string dir)
        {
            ArgumentNullException.ThrowIfNull(dir);

            if (sftp.Exists(dir)) return;

            var index = dir.LastIndexOfAny(new char[] { '/', '\\' });
            if (index > 0)
            {
                var p = dir[..index];
                if (!sftp.Exists(p))
                    CreateDir(sftp, p);
                sftp.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Release object
        /// </summary>
        public void Dispose()
        {
            if (_sftp == null) return;

            if (_sftp.IsConnected)
                _sftp.Disconnect();
            _sftp.Dispose();
        }
    }
}