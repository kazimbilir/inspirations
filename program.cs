using System.Net;

try
{
    var log = Task.Run(() => Console.WriteLine("Start"));
    var httpResponse = log.ContinueWith(async _ =>
        {
            var client = new HttpClient();
            //OnlyOnRanToCompletion
            var result = await client.GetAsync("https://dummyjson.com/products/1");
            //OnlyOnFaulted
            //var result = await client.GetAsync("https://cdummyjson.com/products/1");
            return result;
        }
    );

    httpResponse.ContinueWith(_ => _, TaskContinuationOptions.OnlyOnFaulted);

    var endResult = httpResponse.ContinueWith(t =>
    {
        var result = t.Result;
        return new Tuple<HttpStatusCode, string>(result.Result.StatusCode, result.Result.ToString());
    }, TaskContinuationOptions.OnlyOnRanToCompletion);

    Console.WriteLine($"Http Status: {endResult.Result.Item1}\nContent: {endResult.Result.Item2}");

    log.ContinueWith(_ => Console.WriteLine("End"));
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
