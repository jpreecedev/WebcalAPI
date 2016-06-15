namespace Webcal.Model
{
    using System;

    public class DirectUploadViewModel
    {
        public int DocumentId { get; set; }
        public DateTime Uploaded { get; set; }
        public string FileName { get; set; }
    }
}