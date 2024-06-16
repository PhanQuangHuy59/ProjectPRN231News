using System.Globalization;

namespace FinalProject.Services
{
    public class FileUploadService
    {
        private IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;

        }
        public string UploadFile(IFormFile fileUpload,
            string folder
            )
        {
            string pathDirectory = Path.Combine(_environment.ContentRootPath, folder);

            string pathDirectoryDebug = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            string fileNameSave = Path.GetFileNameWithoutExtension(fileUpload.FileName) + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + Path.GetExtension(fileUpload.FileName);


            var file = Path.Combine(pathDirectory, fileNameSave);

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                fileUpload.CopyTo(fileStream);
            }
            return file;
        }
    }
}
