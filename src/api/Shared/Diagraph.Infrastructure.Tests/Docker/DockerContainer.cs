using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace Diagraph.Infrastructure.Tests.Docker;

public class DockerContainer : IAsyncLifetime
{
    private string _containerId;
    
    private readonly string _containerName;
    private readonly string _imageName;

    private readonly List<string> _env;
    private readonly Dictionary<string, string> _ports;

    private readonly DockerClient _dockerClient;
    
    public Func<Task<bool>> CheckStatus { get; set; }

    public DockerContainer(
        string containerName, 
        string imageName, 
        List<string> env = null, 
        Dictionary<string, string> ports = null)
    {
        _containerName = $"{containerName}-{Guid.NewGuid()}";
        _imageName = imageName;
        _env = env;
        _ports = ports;
        
        Uri dockerUri = new(Environment.OSVersion.Platform == PlatformID.Win32NT 
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock");
        _dockerClient = new DockerClientConfiguration(dockerUri).CreateClient();
    }
    
    private async Task EnsureRunningAsync(Func<Task<bool>> checkStatus)
    {
        Ensure.NotNull(checkStatus);
        
        var runningContainers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());
        if (runningContainers.Any(c => c.ID == _containerId)) return;

        var images = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters());
        if (!images.Any(i => i.RepoTags.Contains(_imageName)))
        {
            string[] imageNameParts = _imageName.Split(':');
            
            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = imageNameParts[0],
                    Tag = imageNameParts[1]
                }, 
                authConfig: null, 
                new Progress<JSONMessage>());
        }

        await StartContainer();
        await WaitUntilContainerUp(checkStatus);
    }

    public async Task RestartAsync(Func<Task<bool>> checkStatus)
    {
        await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
        await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters());
        
        await EnsureRunningAsync(checkStatus);
    }
    
    private async Task StartContainer()
    {
        CreateContainerParameters parameters = new()
        {
            Image = _imageName,
            Name = _containerName
        };

        if (_ports != null)
        {
            parameters.HostConfig = new()
            {
                PortBindings = _ports.ToDictionary(
                    kv => $"{kv.Key}/tcp", 
                    kv => new [] { new PortBinding { HostPort = kv.Value }} as IList<PortBinding>)
            };
        }

        if (_env != null)
        {
            parameters.Env = _env;
        }
        
        CreateContainerResponse result = await _dockerClient.Containers.CreateContainerAsync(parameters);
        _containerId = result.ID;

        await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters());
    }
    
    private async Task WaitUntilContainerUp(Func<Task<bool>> checkStatus)
    {
        DateTime start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 15;
        bool connectionEstablished = false;
        
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                connectionEstablished = await checkStatus();
            }
            catch
            {
                await Task.Delay(50);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception($"Connection to the container {_containerName} could not be established within {maxWaitTimeSeconds} seconds.");
        }
    }
    
    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        if (CheckStatus == null) throw new InvalidOperationException($"Cannot start without {nameof(CheckStatus)} set.");
        await EnsureRunningAsync(CheckStatus);
    }

    public async Task DisposeAsync()
    {
        await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
        await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters());

        var runningVolumes = await _dockerClient.Volumes.ListAsync();
        
        foreach (var volume in runningVolumes.Volumes)
        {
            DateTime createdAt = DateTime.Parse(volume.CreatedAt);
            
            if (createdAt > DateTime.UtcNow.AddMinutes(-5))
            {
                try
                {
                    _dockerClient.Volumes.RemoveAsync(volume.Name)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult(); 
                }
                catch 
                {
                    // Ignore failed attempts to delete volume.
                }
            }
        }
    }
    
    #endregion
}