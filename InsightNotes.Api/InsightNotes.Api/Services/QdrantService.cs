using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Net.Http;

namespace InsightNotes.Api.Services
{
    public class QdrantService
    {

        private readonly QdrantGrpcClient client;

        public QdrantService()
        {
            // connects to http://localhost:6334 (gRPC) by default
            client = new QdrantGrpcClient("localhost");
        }


    }
}
