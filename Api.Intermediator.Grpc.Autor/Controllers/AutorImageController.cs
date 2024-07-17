using Api.Intermediator.Grpc.Autor.Model;
using Google.Protobuf;
using gRPC.Microservicio.Autor;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace Api.Intermediator.Grpc.Autor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorImageController : ControllerBase
    {
        private readonly ILogger<AutorImageController> _logger;

        public AutorImageController(ILogger<AutorImageController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> AddImage(ModelAutor model)
        {
            _logger.LogInformation("Starting AddImage method");

            var channel = GrpcChannel.ForAddress("http://grpc_server:5000", new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });
            var client = new AutorImage.AutorImageClient(channel);
            string modelImage = model.Image;
            byte[] imageBytes = Convert.FromBase64String(modelImage);
            var imageRequest = new AutorIMG
            {
                AutorGuid = model.Guid,
                Image = ByteString.CopyFrom(imageBytes)
            };

            _logger.LogInformation("Sending gRPC request to add image");
            var response = await client.AddAutorIMGAsync(imageRequest);
            _logger.LogInformation("Received gRPC response for AddImage");

            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetImages()
        {
            _logger.LogInformation("Starting GetImages method");

            var list = new List<ModelAutor>();
            var channel = GrpcChannel.ForAddress("http://grpc_server:5000", new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });
            var client = new AutorImage.AutorImageClient(channel);
            var response = await client.GetAutoresIMGAsync(new Empty());
            foreach (var autor in response.Autores)
            {
                var oAutor = new ModelAutor()
                {
                    Guid = autor.AutorGuid,
                    Image = autor.Image.ToBase64()
                };
                list.Add(oAutor);
            }
            var json = new
            {
                request = list
            };

            _logger.LogInformation("Returning images list");
            return new JsonResult(json);
        }

        [HttpGet("{guid}")]
        public async Task<ActionResult> GetImagesById(string guid)
        {
            _logger.LogInformation($"Starting GetImagesById method for guid: {guid}");

            var channel = GrpcChannel.ForAddress("http://grpc_server:5000", new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });
            var client = new AutorImage.AutorImageClient(channel);
            var request = new Autorid
            {
                Id = guid,
            };
            var response = await client.GetAutorIMGByGuidAsync(request);
            var json = new
            {
                Guid = response.AutorGuid,
                Image = response.Image.ToBase64()
            };

            _logger.LogInformation($"Returning image for guid: {guid}");
            return new JsonResult(json);
        }
    }
}
