using Api.Intermediator.Grpc.Autor.Model;
using Google.Protobuf;
using gRPC.Microservicio.Autor;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace Api.Intermediator.Grpc.Autor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorImageController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddImage(ModelAutor model)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7211");
            var client = new AutorImage.AutorImageClient(channel);
            string modelImage = model.Image;
            byte[] imageBytes = Convert.FromBase64String(modelImage);
            var imageRequest = new AutorIMG
            {
                AutorGuid = model.Guid,
                Image = ByteString.CopyFrom(imageBytes)
            };

           

            var response = await client.AddAutorIMGAsync(imageRequest);
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetImages()
        {
            var list = new List<ModelAutor>();
            var channel = GrpcChannel.ForAddress("https://localhost:7211");
            var client = new AutorImage.AutorImageClient(channel);
            var response = await client.GetAutoresIMGAsync(new Empty());
            foreach(var autor in response.Autores)
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
            return new JsonResult(json);
        }

        [HttpGet("{guid}")]
        public async Task<ActionResult> GetImagesById(string guid)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7211");
            var client = new AutorImage.AutorImageClient(channel);
            var request = new Autorid
            {
                Id = guid,
            };
            var response = await client.GetAutorIMGByGuidAsync(request);
            var json = new
            {
                Guid = response.AutorGuid,
                Image = response.Image
            };
            return new JsonResult(json);
        }
    }
}
