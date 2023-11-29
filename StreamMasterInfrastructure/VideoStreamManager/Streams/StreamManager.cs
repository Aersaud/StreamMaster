﻿using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Streams;

public sealed class StreamManager(
    IClientStreamerManager clientStreamerManager,
    IStreamHandlerFactory streamHandlerFactory,
    ILogger<StreamManager> logger
    ) : IStreamManager
{
    private readonly ConcurrentDictionary<string, IStreamHandler> _streamHandlers = new();
    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                // Dispose of each stream controller
                foreach (IStreamHandler streamHandler in _streamHandlers.Values)
                {
                    try
                    {
                        streamHandler.Stop();
                        streamHandler.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error disposing stream handler {VideoStreamId}", streamHandler.VideoStreamId);
                    }
                }

                // Clear the dictionary
                _streamHandlers.Clear();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during disposing of StreamManager");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    private async Task<IStreamHandler?> CreateStreamHandler(VideoStreamDto videoStreamDto, string ChannelName, int rank, CancellationToken cancellation)
    {

        IStreamHandler? streamHandler = await streamHandlerFactory.CreateStreamHandlerAsync(videoStreamDto, ChannelName, rank, cancellation);

        return streamHandler;
    }

    public IStreamHandler? GetStreamHandler(string videoStreamId)
    {
        return !_streamHandlers.TryGetValue(videoStreamId, out IStreamHandler? streamHandler) ? null : streamHandler;
    }

    public async Task<IStreamHandler?> GetOrCreateStreamHandler(VideoStreamDto videoStreamDto, string ChannelName, int rank, CancellationToken cancellation = default)
    {
        _ = _streamHandlers.TryGetValue(videoStreamDto.User_Url, out IStreamHandler? streamHandler);

        if (streamHandler?.IsFailed == true)
        {
            _ = StopAndUnRegisterHandler(videoStreamDto.User_Url);
            _ = _streamHandlers.TryGetValue(videoStreamDto.User_Url, out streamHandler);
        }

        if (streamHandler?.IsFailed != false)
        {
            logger.LogInformation("Creating new handler for stream: {Id} {name}", videoStreamDto.Id, videoStreamDto.User_Tvg_name);
            streamHandler = await CreateStreamHandler(videoStreamDto, ChannelName, rank, cancellation);
            if (streamHandler == null)
            {
                return null;
            }
            _ = _streamHandlers.TryAdd(videoStreamDto.User_Url, streamHandler);

            return streamHandler;
        }

        logger.LogInformation("Reusing handler for stream: {Id} {name}", videoStreamDto.Id, videoStreamDto.User_Tvg_name);
        return streamHandler;
    }

    public IStreamHandler? GetStreamHandlerFromStreamUrl(string streamUrl)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamHandlers.Count(x => x.Value.M3UFileId == m3uFileId);
    }

    public List<IStreamHandler> GetStreamHandlers()
    {
        return _streamHandlers.Values == null ? ([]) : ([.. _streamHandlers.Values]);
    }

    public async Task MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler, CancellationToken cancellationToken = default)
    {
        IEnumerable<Guid> ClientIds = oldStreamHandler.GetClientStreamerClientIds();

        if (!ClientIds.Any())
        {
            return;
        }

        foreach (Guid clientId in ClientIds)
        {
            _ = oldStreamHandler.UnRegisterClientStreamer(clientId);

            IClientStreamerConfiguration? streamerConfiguration = await clientStreamerManager.GetClientStreamerConfiguration(clientId, cancellationToken);
            if (streamerConfiguration == null)
            {
                logger.LogError("Error registering stream configuration for client {ClientId}, streamerConfiguration null.", clientId);
                return;
            }

            newStreamHandler.RegisterClientStreamer(streamerConfiguration);
            await clientStreamerManager.SetClientBufferDelegate(clientId, newStreamHandler.RingBuffer, cancellationToken);
        }

        if (oldStreamHandler.ClientCount == 0)
        {
            _ = StopAndUnRegisterHandler(oldStreamHandler.StreamUrl);
        }
    }

    public IStreamHandler? GetStreamHandlerByClientId(Guid ClientId)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.HasClient(ClientId));
    }

    public bool StopAndUnRegisterHandler(string VideoStreamUrl)
    {
        if (_streamHandlers.TryRemove(VideoStreamUrl, out IStreamHandler? streamHandler))
        {
            streamHandler.Stop();
            return true;
        }

        logger.LogWarning("Failed to remove stream information for {VideoStreamUrl}", VideoStreamUrl);
        return false;
    }
}