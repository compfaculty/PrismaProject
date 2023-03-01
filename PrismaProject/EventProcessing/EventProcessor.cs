using System;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrismaProject.Data;
using PrismaProject.Dto;
using PrismaProject.Models;

namespace PrismaProject.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<EventProcessor> _logger;


    public EventProcessor(ILogger<EventProcessor> logger, IServiceScopeFactory scopeFactory, AutoMapper.IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public void ProcessEvent(string message)
    {
        _logger.LogInformation("--> ProcessEvent!");
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IMessageRepo>();

            var messagePublishDto = JsonSerializer.Deserialize<MessagePublishDto>(message);

            try
            {
                var msg = _mapper.Map<Message>(messagePublishDto);
                repo.CreateAsync(msg).Wait();
                _logger.LogInformation("--> Message added!");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("--> Could not add message to DB {ExMessage}", ex.Message);
            }
        }
    }
}