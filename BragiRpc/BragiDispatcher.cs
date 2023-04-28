namespace BragiRpc;

public class BragiDispatcher {
    public virtual async Task<BaseResponse> AddAsync(BaseRequest request) {
        var addRequest = request as AddRequest;
        return new AddResponse()
        {
            Sum = addRequest.A + addRequest.B,
        };
    }

    public virtual async Task<BaseResponse> EchoAsync(BaseRequest request)
    {
        var echoRequest = request as EchoRequest;
        return new EchoResponse
        {
            Message = echoRequest.Message,
        };
    }

    public virtual async IAsyncEnumerable<BaseResponse> FibAsync(BaseRequest request)
    {
        var fibRequest = request as FibRequest;

        var fib0 = 1;
        var fib1 = 1;

        for (var i = 0;i < 10; i++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1500));
            yield return new FibResponse
            {
                Id = i,
                Value = fib0,
            };

            var t = fib0 + fib1;
            fib1 = fib0;
            fib0 = t;
        }
    }
}
