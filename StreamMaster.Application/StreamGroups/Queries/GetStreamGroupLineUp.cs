﻿using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;
using StreamMaster.SchedulesDirect.Helpers;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupLineup(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupLineupValidator : AbstractValidator<GetStreamGroupLineup>
{
    public GetStreamGroupLineupValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupLineupHandler(IHttpContextAccessor httpContextAccessor, IIconHelper iconHelper, IEPGHelper epgHelper, ISchedulesDirectDataService schedulesDirectDataService, ILogger<GetStreamGroupLineup> logger, IRepositoryWrapper Repository, IMemoryCache memoryCache)
    : IRequestHandler<GetStreamGroupLineup, string>
{
    public async Task<string> Handle(GetStreamGroupLineup request, CancellationToken cancellationToken)
    {
        Setting setting = memoryCache.GetSetting();
        string requestPath = httpContextAccessor.GetUrlWithPathValue();
        byte[]? iv = requestPath.GetIVFromPath(setting.ServerKey, 128);
        if (iv == null)
        {
            return "";
        }

        string url = httpContextAccessor.GetUrl();
        List<SGLineup> ret = [];

        //IEnumerable<VideoStream> videoStreams;
        //if (request.StreamGroupId > 1)
        //{
        //    StreamGroup? streamGroup = await Repository.StreamGroup
        //            .FindAll()
        //            .Include(a => a.ChildVideoStreams)
        //            .FirstOrDefaultAsync(a => a.StreamGroupNumber == request.StreamGroupNumber, cancellationToken: cancellationToken)
        //            .ConfigureAwait(false);

        //    if (streamGroup == null)
        //    {
        //        return "";
        //    }
        //    videoStreams = streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStream).Where(a => !a.IsHidden);
        //}
        //else
        //{
        //    videoStreams = Repository.VideoStream.GetVideoStreamsNotHidden();
        //}

        List<VideoStreamDto> videoStreams = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken);

        if (!videoStreams.Any())
        {
            return JsonSerializer.Serialize(ret);
        }

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();
        foreach (VideoStreamDto videoStream in videoStreams.OrderBy(a => a.User_Tvg_chno))
        {
            if (setting.M3UIgnoreEmptyEPGID && string.IsNullOrEmpty(videoStream.User_Tvg_ID))
            {
                continue;
            }

            bool isDummy = epgHelper.IsDummy(videoStream.User_Tvg_ID);

            if (isDummy)
            {
                videoStream.User_Tvg_ID = $"{EPGHelper.DummyId}-{videoStream.Id}";
                VideoStreamConfig videoStreamConfig = new()
                {
                    Id = videoStream.Id,
                    M3UFileId = videoStream.M3UFileId,
                    User_Tvg_name = videoStream.User_Tvg_name,
                    Tvg_ID = videoStream.Tvg_ID,
                    User_Tvg_ID = videoStream.User_Tvg_ID,
                    User_Tvg_Logo = videoStream.User_Tvg_logo,
                    User_Tvg_chno = videoStream.User_Tvg_chno,
                    IsDuplicate = false,
                    IsDummy = false
                };
                dummyData.FindOrCreateDummyService(videoStream.User_Tvg_ID, videoStreamConfig);
            }

            int epgNumber = EPGHelper.DummyId;
            string stationId;

            if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
            {
                stationId = videoStream.User_Tvg_group;
            }
            else
            {
                if (epgHelper.IsValidEPGId(videoStream.User_Tvg_ID))
                {
                    (epgNumber, stationId) = videoStream.User_Tvg_ID.ExtractEPGNumberAndStationId();
                }
                else
                {
                    stationId = videoStream.User_Tvg_ID;
                }

            }
            string encodedNumbers = request.StreamGroupId.EncodeValues128(videoStream.Id, setting.ServerKey, iv);

            string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim().Replace(" ", "_");
            string videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            MxfService? service = schedulesDirectDataService.AllServices.GetMxfService(videoStream.User_Tvg_ID);
            if (service == null)
            {
                continue;
            }
            string graceNote = service?.CallSign ?? stationId;
            string id = graceNote;
            if (setting.M3UUseChnoForId)
            {
                id = videoStream.User_Tvg_chno.ToString();
            }

            string? logo = "";
            if (service != null && service.mxfGuideImage != null && !string.IsNullOrEmpty(service.mxfGuideImage.ImageUrl))
            {
                logo = service.mxfGuideImage.ImageUrl;
                string _baseUrl = httpContextAccessor.GetUrl();
                logo = iconHelper.GetIconUrl(service.EPGNumber, service.extras["logo"].Url, _baseUrl);
            }

            SGLineup lu = new()
            {
                GuideName = videoStream.User_Tvg_name,
                GuideNumber = id,
                Station = id,
                Logo = logo,
                URL = videoUrl
            };

            ret.Add(lu);
        }

        string jsonString = JsonSerializer.Serialize(ret);
        return jsonString;
    }
}