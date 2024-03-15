namespace ProductService.Models
{
    public class MyResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
}
