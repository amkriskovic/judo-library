using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using JudoLibrary.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JudoLibrary.Api.BackgroundServices
{
    public class VideoEditingBackgroundService : BackgroundService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<VideoEditingBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ChannelReader<EditVideoMessage> _channelReader;

        public VideoEditingBackgroundService(
            IWebHostEnvironment environment,
            ILogger<VideoEditingBackgroundService> logger,
            Channel<EditVideoMessage> channel,
            IServiceProvider serviceProvider)
        {
            _environment = environment;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _channelReader = channel.Reader;
        }
        
        // This method is called when the IHostedService starts which has implementation of AddHostedService
        // The implementation should return a task that represents the lifetime of the long running operation(s) being performed.
        // * Idea is for this Task to be looping
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Waiting until something is ready to be read from a channel
            // If true, we enter while loop, if false, channel has been closed
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                // Grab the message from a channel -> object that is constructed in SubmissionsController
                var message = await _channelReader.ReadAsync(stoppingToken);
                
                // Try/Cath to now blow up program
                try
                {
                    // Creating input path, wwwroot + message input that is video string <- original
                    var inputPath = Path.Combine(_environment.WebRootPath, message.Input);
                    
                    // Creating output name for a video to be able to access it
                    var outputName = $"converted_{DateTime.Now.Ticks}.mp4";

                    // Creating output path, wwwroot + outputName that is custom "unique" converted string
                    var outputPath = Path.Combine(_environment.WebRootPath, outputName);

                    // Object for starting process,
                    // Specifies a set of values that are used when you start a process.
                    var startInfo = new ProcessStartInfo
                    {
                        // Gets or sets the application or document to start.
                        // Getting ffmpeg executable which provides CLI tools
                        FileName = Path.Combine(_environment.ContentRootPath, "ffmpeg", "ffmpeg.exe"),

                        // Gets or sets the set of command-line arguments to use when starting the application.
                        // Specifying arguments for manipulating video,
                        // inputPath -> video location with name {original}
                        // outputPath ->  video location with name {converted}
                        Arguments = $"-y -i {inputPath} -an -vf scale=640x480 {outputPath}",

                        // Gets or sets the working directory for the process to be started
                        // Specifying where things are happening
                        WorkingDirectory = _environment.WebRootPath,

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
                    
                    // Create scope
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // Get AppDbContext from scope
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        
                        // Get submission where submission Id is === to message from channel that -> SubmissionId
                        var submission = context.Submissions.FirstOrDefault(s => s.Id.Equals(message.SubmissionId));
                        
                        // Assign outputName to submission.Video, assigning converted video to original one
                        submission.Video = outputName;
                        
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
                }
            }

        }
    }
    
    // Class that is going to be passed between processes, with options of SubmissionId & Input
    public class EditVideoMessage
    {
        public int SubmissionId { get; set; }
        public string Input { get; set; }
    }
}