// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

global using Admin.NET.Core.Service;
global using Furion;
global using Furion.ConfigurableOptions;
global using Furion.DatabaseAccessor;
global using Furion.DataEncryption;
global using Furion.DataValidation;
global using Furion.DependencyInjection;
global using Furion.DynamicApiController;
global using Furion.EventBus;
global using Furion.FriendlyException;
global using Furion.HttpRemote;
global using Furion.JsonSerialization;
global using Furion.Logging;
global using Furion.Schedule;
global using Furion.UnifyResult;
global using Furion.ViewEngine;
global using Magicodes.ExporterAndImporter.Core;
global using Magicodes.ExporterAndImporter.Core.Extension;
global using Magicodes.ExporterAndImporter.Excel;
global using Mapster;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using NewLife;
global using NewLife.Caching;
global using Newtonsoft.Json.Linq;
global using SKIT.FlurlHttpClient;
global using SKIT.FlurlHttpClient.Wechat.Api;
global using SKIT.FlurlHttpClient.Wechat.Api.Models;
global using SKIT.FlurlHttpClient.Wechat.TenpayV3;
global using SKIT.FlurlHttpClient.Wechat.TenpayV3.Events;
global using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
global using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
global using SqlSugar;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Data;
global using System.Diagnostics;
global using System.Linq.Dynamic.Core;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Web;
global using UAParser;
global using Yitter.IdGenerator;