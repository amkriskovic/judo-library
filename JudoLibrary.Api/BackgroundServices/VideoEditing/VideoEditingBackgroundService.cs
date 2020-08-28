using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JudoLibrary.Api.BackgroundServices.VideoEditing
{
    public class VideoEditingBackgroundService : BackgroundService
    {
        private readonly ILogger<VideoEditingBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly VideoManager _videoManager;
        private readonly ChannelReader<EditVideoMessage> _channelReader;

        public VideoEditingBackgroundService(
            ILogger<VideoEditingBackgroundService> logger,
            Channel<EditVideoMessage> channel,
            IServiceProvider serviceProvider,
            VideoManager videoManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _videoManager = videoManager;
            _channelReader = channel.Reader;
        }
        
        // This method is called when the IHostedService starts which has implementation of AddHostedService
        // The implementation should return a task that represents the lifetime of the long running operation(s) being performed.
        // * Idea is for this Task to be looping
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // * Channel consumer
            // aWaiting until something is ready to be read from a channel
            // If true, we enter while loop , if false -> channel has been closed -> exiting while loop
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                // Grab the message from a channel -> modItem/msg that is constructed/has been written in SubmissionsController -> POST
                var message = await _channelReader.ReadAsync(stoppingToken);
                
                // Creating input path, wwwroot + message that is video string <- original from channel
                var inputPath = _videoManager.TemporarySavePath(message.Input);

                // Creating converted output name for a video to be able to access it
                var outputConvertedName = _videoManager.GenerateConvertedFileName();
                    
                // Creating thumbnail output name for a video
                var outputThumbnailName = _videoManager.GenerateThumbnailFileName();

                // Creating output converted path based on output video name
                var outputConvertedPath = _videoManager.TemporarySavePath(outputConvertedName);
                    
                // Creating output thumbnail path based on output video name
                var outputThumbnailPath = _videoManager.TemporarySavePath(outputThumbnailName);
                
                // Try/Cath to now blow up program
                try
                {
                    // Object for starting process,
                    // Specifies a set of values that are used when you start a process.
                    var startInfo = new ProcessStartInfo
                    {
                        // Gets or sets the application or document to start.
                        // Getting ffmpeg executable which provides CLI tools
                        FileName = _videoManager.FFMPEGPath,

                        // Gets or sets the set of command-line arguments to use when starting the application.
                        // Specifying arguments for manipulating video,
                        // inputPath -> video location with name {original}
                        // outputPath ->  video location with name {converted}
                        // after second path we create thumbnail for video at 00:00:00 time and scale it to same size as video, with random name, save it to path with converted video
                        Arguments = $"-y -i {inputPath} -an -vf scale=540x380 {outputConvertedPath} -ss 00:00:00 -vframes 1 -vf scale=540x380 {outputThumbnailPath}",

                        // Gets or sets a value indicating whether to start the process in a new window.
                        CreateNoWindow = true,

                        // Gets or sets a value indicating whether to use the operating 
                        // system shell to start the process
                        UseShellExecute = false
                    };

                    // Provides access to local and remote processes and enables you to start and stop local system processes.
                    // Creating new Process object and assigning startInfo information's to prop of Process class
                    using (var process = new Process {StartInfo = startInfo})
                    {
                        process.Start();
                        process.WaitForExit();
                    }

                    // If temporary converted video does NOT exist -> problem!!
                    if (!_videoManager.TemporaryFileExists(outputConvertedName))
                    {
                        // Throw an exception
                        throw new Exception("FFMPEG failed to generate converted video!");
                    }
                    
                    // If thumbnail for a video does NOT exist -> problem!!
                    if (!_videoManager.TemporaryFileExists(outputThumbnailName))
                    {
                        // Throw an exception
                        throw new Exception("FFMPEG failed to generate thumbnail for a video!");
                    }

                    // Create scope, after using is finished, we get disposed of service that we injected
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // Get AppDbContext from scope
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        // Get submission where submission Id is === to message from channel that -> SubmissionId
                        var submission = context.Submissions.FirstOrDefault(s => s.Id.Equals(message.SubmissionId));

                        // Assign outputConvertedName & outputThumbnailName to VideoLink & ThumbLink to Submission
                        // which contains Video prop inside itself
                        submission.Video = new Video
                        {
                            ThumbLink = outputThumbnailName,
                            VideoLink = outputConvertedName
                        };

                        // Once video is updated -> mark video as Processed
                        // Until then it will be hidden
                        submission.VideoProcessed = true;

                        // Save changes to DB
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Video processing has failed for {message.Input}");
                    
                    // Catch if some of these failed
                    _videoManager.DeleteTemporaryFileInPath(outputConvertedName);
                    _videoManager.DeleteTemporaryFileInPath(outputThumbnailName);
                }
                finally
                {
                    // finally block run when control leaves a try statement.
                    // * This is responsible for deleting temp video when everything went fine, when we pressed Save on the form
                    // Finally -> no matter what happens =>> delete temporary video based on channel message input
                    _videoManager.DeleteTemporaryFileInPath(message.Input);
                }
            }

        }
    }
    
}