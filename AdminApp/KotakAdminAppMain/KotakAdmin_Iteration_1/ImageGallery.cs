using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;

namespace KotakAdmin_Iteration_1
{
    public class ImageGallery
    {
        private string ConStr { get; set; }
        private string DbName { get; set; }

        public ImageGallery(string ConStr, string DbName)
        {
            this.ConStr = ConStr;
            this.DbName = DbName;
        }

        private GridFSBucket Open()
        {
            return new GridFSBucket(new MongoClient(this.ConStr).GetDatabase(this.DbName));
        }
        public string Set(string CRN,byte[] img)
        {
            CRN+="_"+DateTime.Now.ToString("ddMMyyyy_HHmmss_ffffff");
            GridFSBucket bucket = Open();
            return bucket.UploadFromBytes(CRN, img).ToString();
        }

        public byte[] Get(string fileId)
        {
            GridFSBucket bucket = Open();
            ObjectId id = ObjectId.Parse(fileId);
            return bucket.DownloadAsBytes(id);
        }
        public void Drop(string fileId)
        {
            GridFSBucket bucket = Open();
            ObjectId id = ObjectId.Parse(fileId);
            bucket.Delete(id);
        }

    }
 
}
