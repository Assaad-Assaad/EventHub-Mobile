using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Utils
{
    //public class LoggingHandler : DelegatingHandler
    //{
    //    public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    //    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        // Log the request URL and headers
    //        Console.WriteLine("Request URL: " + request.RequestUri);
    //        Console.WriteLine("Request Method: " + request.Method);
    //        Console.WriteLine("Request Headers: " + string.Join(", ", request.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));

    //        // Log the request body (if any)
    //        if (request.Content != null)
    //        {
    //            var content = await request.Content.ReadAsStringAsync();
    //            Console.WriteLine("Request Body: " + content);
    //        }

    //        var response = await base.SendAsync(request, cancellationToken);

    //        // Log the response status and body
    //        Console.WriteLine("Response Status: " + response.StatusCode);
    //        var responseBody = await response.Content.ReadAsStringAsync();
    //        Console.WriteLine("Response Body: " + responseBody);

    //        return response;
    //    }
    //}

}
