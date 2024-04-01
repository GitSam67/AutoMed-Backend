namespace AutoMed_Backend.Models
{
    public class ResponseObject<TEntity> where TEntity : class
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class CollectionResponse<TEntity> : ResponseObject<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity>? Records { get; set; }
    }


    public class SingleObjectResponse<TEntity> : ResponseObject<TEntity> where TEntity : class
    {
        public TEntity? Record { get; set; }
    }
}
