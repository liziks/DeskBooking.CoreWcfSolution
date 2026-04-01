
using Microsoft.Extensions.Options;
using DeskBooking.WebClient.Infrastructure;
using System.ServiceModel;
using System.Xml;

namespace DeskBooking.WebClient.Services;

public class SoapClientExecutor
{
    private readonly SoapEndpointOptions _options;

    public SoapClientExecutor(IOptions<SoapEndpointOptions> options)
    {
        _options = options.Value;
    }

    public string AuthServiceUrl => _options.AuthServiceUrl;
    public string RoomServiceUrl => _options.RoomServiceUrl;
    public string BookingServiceUrl => _options.BookingServiceUrl;

    public async Task<TResult> ExecuteAsync<TContract, TResult>(
        string endpointUrl,
        Func<TContract, Task<TResult>> action)
        where TContract : class
    {
        var binding = new BasicHttpBinding
        {
            MaxReceivedMessageSize = int.MaxValue,
            ReaderQuotas = XmlDictionaryReaderQuotas.Max
        };

        var factory = new ChannelFactory<TContract>(binding, new EndpointAddress(endpointUrl));
        var channel = factory.CreateChannel();
        var communicationObject = (ICommunicationObject)channel;

        try
        {
            var result = await action(channel);
            communicationObject.Close();
            factory.Close();
            return result;
        }
        catch
        {
            communicationObject.Abort();
            factory.Abort();
            throw;
        }
    }
}
