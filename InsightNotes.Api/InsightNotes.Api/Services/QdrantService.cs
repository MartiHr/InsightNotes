using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace InsightNotes.Api.Services
{
    public class QdrantService : IDisposable
    {
        private readonly QdrantClient client;
        private const string CollectionName = "notes";
        private bool disposed = false;

        public QdrantService()
        {
            client = new QdrantClient("localhost");
        }

        public async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                // Check if collection already exists
                if (await CollectionExistsAsync())
                {
                    return;
                }

                var vectorParams = new VectorParams
                {
                    Size = 1536,
                    Distance = Distance.Cosine
                };

                await client.CreateCollectionAsync(CollectionName, vectorParams);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create collection: {ex.Message}", ex);
            }
        }

        public async Task RecreateCollectionAsync()
        {
            try
            {
                // Delete collection if it exists
                if (await CollectionExistsAsync())
                {
                    await client.DeleteCollectionAsync(CollectionName);
                }

                var vectorParams = new VectorParams
                {
                    Size = 1536,
                    Distance = Distance.Cosine
                };

                await client.CreateCollectionAsync(CollectionName, vectorParams);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to recreate collection: {ex.Message}", ex);
            }
        }

        public async Task StoreNoteVectorAsync(Guid noteId, float[] vector, string title, string content)
        {
            if (vector == null || vector.Length == 0)
                throw new ArgumentException("Vector cannot be null or empty", nameof(vector));

            try
            {
                var point = new PointStruct
                {
                    Id = new PointId { Uuid = noteId.ToString() },
                    Vectors = vector,
                    Payload =
                    {
                        ["title"] = title ?? string.Empty,
                        ["content"] = content ?? string.Empty,
                        ["createdAt"] = DateTime.UtcNow.ToString("o"),
                        ["noteId"] = noteId.ToString()
                    }
                };

                await client.UpsertAsync(CollectionName, new[] { point });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to store note vector: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<ScoredPoint>> GetAllPointsAsync()
        {
            var allPoints = new List<ScoredPoint>();
            PointId? nextPageOffset = null; // Updated type to match the expected 'PointId?'
            const uint pageSize = 100;

            do
            {
                var pointsResponse = await client.ScrollAsync(
                    CollectionName,
                    limit: pageSize,
                    offset: nextPageOffset // Updated to use 'PointId?' type
                );

                if (pointsResponse.Result.Count == 0) // Updated to use 'Result' property of 'ScrollResponse'
                    break;

                allPoints.AddRange(pointsResponse.Result.Select(p => new ScoredPoint
                {
                    Id = p.Id,
                    Payload = { p.Payload }, // Fixed: Use the `Add` method to populate the read-only `Payload` property
                    Vectors = p.Vectors
                }));

                nextPageOffset = pointsResponse.NextPageOffset; // Ensure 'NextPageOffset' is compatible with 'PointId?'
            }
            while (nextPageOffset != null);

            return allPoints.AsReadOnly();
        }


        public async Task<IReadOnlyList<ScoredPoint>> SearchAsync(float[] queryVector, int limit = 5)
        {
            if (queryVector == null || queryVector.Length == 0)
                throw new ArgumentException("Query vector cannot be null or empty", nameof(queryVector));

            if (limit <= 0)
                throw new ArgumentException("Limit must be greater than 0", nameof(limit));

            try
            {
                var results = await client.SearchAsync(
                    CollectionName,
                    queryVector,
                    limit: (ulong)limit
                );

                return results.ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to search vectors: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<ScoredPoint>> SearchWithFilterAsync(
             float[] queryVector,
             string filterField,
             string filterValue,
             int limit = 5)
        {
            if (queryVector == null || queryVector.Length == 0)
                throw new ArgumentException("Query vector cannot be null or empty", nameof(queryVector));

            if (string.IsNullOrWhiteSpace(filterField))
                throw new ArgumentException("Filter field cannot be null or empty", nameof(filterField));

            if (string.IsNullOrWhiteSpace(filterValue))
                throw new ArgumentException("Filter value cannot be null or empty", nameof(filterValue));

            if (limit <= 0)
                throw new ArgumentException("Limit must be greater than 0", nameof(limit));

            try
            {
                var filter = new Filter
                {
                    Must = {
                        new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = filterField,
                                Match = new Match
                                {
                                    Text = filterValue
                                }
                            }
                        }
                    }
                };

                var results = await client.SearchAsync(
                    CollectionName,
                    queryVector,
                    filter: filter,
                    limit: (ulong)limit
                );

                return results.ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to search vectors with filter: {ex.Message}", ex);
            }
        }

        public async Task DeleteNoteAsync(Guid noteId)
        {
            if (noteId == Guid.Empty)
                throw new ArgumentException("Note ID cannot be empty", nameof(noteId));

            try
            {
                var pointId = new PointId { Uuid = noteId.ToString() };
                await client.DeleteAsync(CollectionName, new[] { pointId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete note: {ex.Message}", ex);
            }
        }

        public async Task<bool> CollectionExistsAsync()
        {
            try
            {
                await client.GetCollectionInfoAsync(CollectionName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ulong> GetPointCountAsync()
        {
            try
            {
                return await client.CountAsync(CollectionName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get point count: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    client?.Dispose();
                }
                disposed = true;
            }
        }
    }
}