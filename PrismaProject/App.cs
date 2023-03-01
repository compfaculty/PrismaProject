using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PrismaProject.AsyncDataServices;
using PrismaProject.Data;
using PrismaProject.Dto;
using PrismaProject.Models;
using PrismaProject.Services;

namespace PrismaProject;

public class App
{
    private readonly ILogger<App> _logger;
    private readonly IProducerService _producer;
    private readonly IMessageRepo _repo;
    private readonly IMessageBusClient _rabbit;
    private readonly IConfiguration _configuration;
    private const int Timeout = 5;

    public CancellationToken TokenTimeout
    {
        get
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            cts.CancelAfter(TimeSpan.FromSeconds(Timeout));
            return token;
        }
    }

    public CancellationToken TokenCancel
    {
        get
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            cts.Cancel();
            return token;
        }
    }

    public App(ILogger<App> logger, IConfiguration configuration, IProducerService producer, IMessageRepo repo,
        IMessageBusClient rabbit)
    {
        _logger = logger;
        _producer = producer;
        _repo = repo;
        _rabbit = rabbit;
        _configuration = configuration;
    }

    public async Task SendMessage(string message)
    {
        _rabbit.PublishNewMessage(new MessagePublishDto() { Msg = message });
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    public async Task GetMessages()
    {
        _logger.LogInformation("GetMessages...");
        var rMsg = await _repo.GetAsync().ConfigureAwait(false);
        _logger.LogInformation($"Got {rMsg.Count}");
        foreach (var m in rMsg)
        {
            _logger.LogInformation($"Got {m.Id} {m.Msg}");
        }
    }
}


/*
        public async Task Start()
        {
            await _crawler.ParseBaseUrlsFromConfig("http://ukr.net", TokenTimeout);
            _logger.LogInformation("Start Scan...");
            Task<string>[] checkedUrls = _network.TargetUrls.Select(url => _crawler.GetUrl(url, TokenTimeout))
                .Distinct().ToArray();
            var processingTasks = checkedUrls.Select(async t =>
            {
                var result = await t;
                _logger.LogInformation(result);
            }).ToArray();

            await Task.WhenAll(processingTasks);
        }

        public async Task Ping()
        {
            _logger.LogInformation("Ping scan!");
            Task[] pingetHosts = _network.TargetUrls
                .Select(url => url.UrlToHostname())
                .Where(host => host != String.Empty)
                .Distinct()
                .Select(target => _pinger.Ping(target, TokenTimeout))
                .ToArray();

//            var processingPings = pingetHosts.Select(async t =>
//            {
//                var result = await t;
//                _logger.LogInformation($"ping result {result}");
//            }).ToArray();

            await Task.WhenAll(pingetHosts);
        }

        public void InitTargets()
        {
            _logger.LogInformation($"Start init targets...");
            Task[] baseparsedurls = _network.BaseUrls.Select(url => _crawler.ParseBaseUrlsFromConfig(url, TokenTimeout))
                .ToArray();
            Task.WaitAll(baseparsedurls);
            _logger.LogInformation($"the number of urls is {_network.TargetUrls.Count}");
        }

        public void InitTargets2()
        {
            _logger.LogInformation($"Start init targets...");
            _network.BaseUrls.AsParallel();

            _logger.LogInformation($"the number of urls is {_network.TargetUrls.Count}");
        }


    }
    
*/