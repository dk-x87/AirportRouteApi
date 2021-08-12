namespace AirportRouteApi.Models
{
    public class Responce<T>
    {
        public T Data { get; set; }
        public Error Error { get; set; }

        public Responce(T data)
        {
            Data = data;
        }

        public Responce(Error error)
        {
            Error = error;
        }
    }
}
