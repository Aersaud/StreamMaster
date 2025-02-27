﻿global using AutoMapper;

global using MediatR;

global using Microsoft.AspNetCore.SignalR;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Logging;

global using StreamMaster.Application.Common;
global using StreamMaster.Application.Common.Events;
global using StreamMaster.Application.Common.Interfaces;
global using StreamMaster.Application.Hubs;
global using StreamMaster.Application.Settings.Queries;
global using StreamMaster.Domain.Attributes;
global using StreamMaster.Domain.Cache;
global using StreamMaster.Domain.Common;
global using StreamMaster.Domain.Dto;
global using StreamMaster.Domain.Enums;
global using StreamMaster.Domain.Extensions;
global using StreamMaster.Domain.Logging;
global using StreamMaster.Domain.Models;
global using StreamMaster.Domain.Repository;
global using StreamMaster.Domain.Services;
global using StreamMaster.SchedulesDirect.Domain.Interfaces;
global using StreamMaster.SchedulesDirect.Domain.JsonClasses;
global using StreamMaster.SchedulesDirect.Domain.Models;
global using StreamMaster.SchedulesDirect.Domain.XmltvXml;
global using StreamMaster.Streams.Domain.Interfaces;
global using StreamMaster.Streams.Domain.Models;

global using X.PagedList;