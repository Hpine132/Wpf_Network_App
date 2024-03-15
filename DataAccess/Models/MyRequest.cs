namespace ProductService.Models
{
    public class MyRequest<T>
    {
        public string RequestType { get; set; }
        public T? Data { get; set; }
    }
}
