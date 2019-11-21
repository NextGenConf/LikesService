namespace LikesApi.Services
{
    using System.Collections.Generic;
    using Gremlin.Net.Driver;
    using Gremlin.Net.Structure;
    using Gremlin.Net.Driver.Remote;
    using Gremlin.Net.Process.Traversal;
    using static Gremlin.Net.Process.Traversal.AnonymousTraversalSource;
    using System.Linq;

    public class LikesService
    {
        private GremlinClient gremlinClient;
        private GraphTraversalSource graph;

        private const string ConferenceLabel = "conference";
        private const string SessionLabel = "session";
        private const string UserLabel = "user";
        private const string EdgeLabel = "likes";
        private const string PropertyKey = "id";

        public LikesService()
        {           
            gremlinClient = new GremlinClient(new GremlinServer("localhost", 8182));

            // TODO: Just temporatry, remove when the real graph is used
            this.CreateDummyGraph();
        }

        public void AddNewLikeForConference(string userId, string conferenceId)
        {
            this.AddNewLike(userId, conferenceId, ConferenceLabel);        
        }

        public void AddNewLikeForSession(string userId, string sessionId)
        {
            this.AddNewLike(userId, sessionId, SessionLabel);
        }

        public List<string> GetConferencesPerUser(string userId)
        {
            var conferences = GetItems(userId, UserLabel, ConferenceLabel, getInEdges: false);
            return conferences;
        }

        public List<string> GetSessionsPerUser(string userId)
        {
            var sessions = GetItems(userId, UserLabel, SessionLabel, getInEdges: false);
            return sessions;
        }

        public List<string> GetUsersPerConference(string conferenceId)
        {
            var users = GetItems(conferenceId, ConferenceLabel, UserLabel, getInEdges: true);
            return users;
        }

        public List<string> GetUsersPerSession(string sessionId)
        {
            var users = GetItems(sessionId, SessionLabel, UserLabel, getInEdges: true);
            return users;
        }

        private void AddNewLike(string userId, string itemId, string label)
        {
            // execute remotely 
            graph = Traversal().WithRemote(new DriverRemoteConnection(gremlinClient));

            // Add vertices if do not exist
            var u = graph.V().Has(UserLabel, PropertyKey, userId);
            var i = graph.V().Has(label, PropertyKey, itemId);
            Vertex user = u.HasNext() ? u.Next() : graph.AddV(UserLabel).Property(PropertyKey, userId).Next();
            Vertex item = i.HasNext() ? i.Next() : graph.AddV(label).Property(PropertyKey, itemId).Next();

            // Add the edge if does not exist
            if (graph.V(user.Id).OutE("likes").ToList().Where(e => e.InV.Id.Equals(item.Id)).Count() == 0)
            {
                graph.V(user).AddE(EdgeLabel).To(item).Iterate();
            }
        }

        private List<string> GetItems(string id, string labelId, string labelItems, bool getInEdges = false)
        {
            graph = Traversal().WithRemote(new DriverRemoteConnection(gremlinClient));

            IList<VertexProperty> vertices = new List<VertexProperty>(0);

            if (getInEdges)
            {
                vertices = graph.V().Has(labelId, PropertyKey, id).In(EdgeLabel).HasLabel(labelItems)
                    .Properties<VertexProperty>(PropertyKey).ToList();
            }
            else
            {
                vertices = graph.V().Has(labelId, PropertyKey, id).Out(EdgeLabel).HasLabel(labelItems)
                    .Properties<VertexProperty>(PropertyKey).ToList();
            }

            var items = new List<string>(0);

            foreach (var v in vertices)
            {
                items.Add(v.Value);
            }

            return items;
        }

        private void CreateDummyGraph()
        {
            AddNewLikeForConference("olwheele@microsoft.com", "JavaScript");
            AddNewLikeForConference("olwheele@microsoft.com", "Services");
            AddNewLikeForConference("mabakovi@microsoft.com", "Services");
            AddNewLikeForSession("olwheele@microsoft.com", "SessionA");
            AddNewLikeForSession("mabakovi@microsoft.com", "SessionB");
            AddNewLikeForSession("mabakovi@microsoft.com", "SessionC");
        }
    }
}
