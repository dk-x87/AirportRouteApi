namespace AirportRouteApi.Models
{
    public class Responce<T>
    {
        public T Data { get; private set; }
        public Error Error { get; private set; }

        public static Responce<T> Success(T data)
        {
            return new Responce<T>() { Data = data};
        }

        public static Responce<T> Fault(Error error)
        {
            return new Responce<T>() { Error = error };
        }

    }
}
