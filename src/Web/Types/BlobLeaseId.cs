namespace Web.Types
{
    public class BlobLeaseId : TinyType<BlobLeaseId, string>
    {
        public BlobLeaseId(string value) : base(value)
        {
        }
    }
}