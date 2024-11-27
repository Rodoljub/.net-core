using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Core.Mapping.Services;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Services;
using Quantum.Core.Services.Contracts;
using Quantum.Data;
using Quantum.Data.Repositories;
using Quantum.Data.Repositories.Contracts;
using Quantum.ExternalIntegration.Services;
using Quantum.Integration.Internal.Services;
using Quantum.Integration.Internal.Services.Contracts;
using Quantum.Integration.Services.Contracts;
using Quantum.Utility.Services;
using Quantum.Utility.Services.Contracts;
using System;
using Quantum.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Quantum.Data.Repositories.Common.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Core.Services.Auth.Contracts;
using Quantum.Core.Services.Auth;
using Quantum.Core.Mapping.Profiles;
using Quantum.Integration.External.Services.Contracts;
using Quantum.Integration.External.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Quantum.Core.SignalR;
using Quantum.Core.Services.ImageSharp.Contracts;
using Quantum.Core.Services.ImageSharp;
using Quartz;
using Quantum.Core.Workers;

namespace Quantum.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServiceQuantumCore(this IServiceCollection services, IConfiguration _config)
        {
            #region Repository Configuration
            services.AddDbContext<QDbContext>(ServiceLifetime.Scoped);
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IFileDetailsRepository, FileDetailsRepository>();
            services.AddScoped<IFileTypeRepository, FileTypeRepository>();
            services.AddScoped<ICLRTypeRepository, CLRTypeRepository>();
            services.AddScoped<IItemsListRepository, ItemsListRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemTagRepository, ItemTagRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            services.AddScoped<IViewRepository, ViewRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IReportedContentReasonRepository, ReportedContentReasonRepository>();
            services.AddScoped<IReportedContentRepository, ReportedContentRepository>();
            services.AddScoped<ISaveSearchResultsRepository, SaveSearchResultsRepository>();
            services.AddScoped<ISaveSearchTagsRepository, SaveSearchTagsRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            #endregion Repository Configuration

            #region Service Configuration
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDocumentService, DocumentService>();

            services.AddScoped<IBaseRepository<Folder, IdentityUser>, BaseRepository<Folder>>();


            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IMappingUserProfileService, MappingUserProfileService>();
            services.AddScoped<IMappingFileService, MappingFileService>();
            services.AddScoped<IMappingItemsService, MappingItemsService>();
            services.AddScoped<IMappingImageService, MappingImageService>();
            services.AddScoped<IMappingCommentService, MappingCommentService>();
            services.AddScoped<IMappingReportedContentService, MappingReportedContentService>();
            services.AddScoped<IMappingSearchService, MappingSearchService>();
            services.AddScoped<IItemsService, ItemsService>();
            services.AddScoped<IItemsListService, ItemsListService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ICLRTypeService, CLRTypeService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IImageSharpService, ImageSharpService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileTypeService, FileTypeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFavouriteService, FavouriteService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IReportedContentService, ReportedContentService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddSevicesQuantumUtility();

            services.AddHttpClient<IWebServerService, WebServerService>("save-image-file", client =>
            {
                client.BaseAddress = new Uri(_config["FrontApp:Domain"]);
            });
            services.AddTransient<IWebServerService, WebServerService>();


            services.AddScoped<IComputerVisionClient, ComputerVisionClient>((serviceProvider) =>
            {
                //ConfigService configService = serviceProvider.GetService<ConfigService>();
                return new ComputerVisionClient(
                    new ApiKeyServiceClientCredentials(_config["Application:ComputerVision:SubscriptionKey2"]),
                    Array.Empty<System.Net.Http.DelegatingHandler>())
                {
                    Endpoint = _config["Application:ComputerVision:AnalyzeImage:Endpoint"]
                };
            });

            services.AddScoped<IComputerVisionService, ComputerVisionService>();
            services.AddScoped<IBlobsStorageService, BlobsStorageService>();

            services.AddAzureClients(builder =>
            {
                // Add a KeyVault client
                //builder.AddSecretClient(keyVaultUrl);

                // Add a storage account client
                builder.AddBlobServiceClient(_config["Application:AzureBlob:ConnectionString"]);

                // Use the environment credential by default
                //builder.UseCredential(new EnvironmentCredential());
            });

            #region AutoMapper

            var mapConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommentsMappingProfile>();
                cfg.AddProfile<FavouritesMappingProfile>();
                cfg.AddProfile<FileDetailsMappingProfile>();
                cfg.AddProfile<FilesMappingProfile>();
                cfg.AddProfile<ItemsMappingProfile>();
                cfg.AddProfile<LikesMappingProfile>();
                cfg.AddProfile<ReportedContentMappingProfile>();
                cfg.AddProfile<SearchMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<UserProfileMappingProfile>();
                //cfg.AddProfile<NodeTypeGroupMappingProfile>();
                //cfg.AddProfile<NodeTypeMappingProfile>();
                //cfg.AddProfile<NodeInstanceMappingProfile>();
            });

            IMapper mapper = mapConfig.CreateMapper();

            services.AddSingleton(mapper);

            #endregion

            // var cert = new X509Certificate2("./example.pfx", "exportpassword");


            //services.AddGraphQLServer()
            //    .AddType<ItemType>()
            //    .AddType<UserProfileType>()
            //    .AddType<GraphFileType>()
            //    .AddQueryType<ItemsQuery>()
            //    .SetPagingOptions(
            //    new PagingOptions()
            //    {
            //        IncludeTotalCount = true
            //    });

            #region Quarz setup

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                // Just use the name of your job that you created in the Jobs folder.
                var jobKey = new JobKey(nameof(ImageProcessorWorker));

                q.AddJob<ImageProcessorWorker>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{nameof(ImageProcessorWorker)}-trigger")
                    //This Cron interval can be described as "run every minute" (when second is zero)
                    .WithCronSchedule(_config["ImageProcessorWorker:Schedule"]));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            #endregion

            //services.AddHostedService<BatchProcessImagesService>();
            services.AddHostedService<SignalRWorker>();

            #endregion Service Configuration

            return services;
        }
    }
}
