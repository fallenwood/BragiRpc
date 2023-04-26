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
}
