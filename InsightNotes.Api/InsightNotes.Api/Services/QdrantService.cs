using Qdrant.Client.Grpc;

namespace InsightNotes.Api.Services
{
    public class QdrantService
    {

        private readonly QdrantGrpcClient client;

        public QdrantService(QdrantGrpcClient client)
        {
            this.client = client;
        }


    }
}
