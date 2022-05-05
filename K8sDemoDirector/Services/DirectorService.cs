using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Workers;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;
using K8sCore.Interfaces.Mongo;
using K8sCore.Messages;
using K8sDemoDirector.Interfaces;
using K8sDemoDirector.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoDirector.Services
{
    public class DirectorService : CyclicWorkerService
    {
        private readonly IServiceProvider _serviceProvider;



        public DirectorService(IServiceProvider serviceProvider, IRabbitConnector rabbitConnector, ILogger logger, IJob job, int cycleTime)
        : base(serviceProvider, rabbitConnector, logger, cycleTime, job)
        {
            _rabbitConnector.Publish<DirectorStartedMessage>(new DirectorStartedMessage());
        }



    }
}