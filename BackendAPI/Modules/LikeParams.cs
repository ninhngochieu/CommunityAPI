using System;

namespace BackendAPI.Modules
{
    public class LikeParams: PaginationParams
    {
        internal Guid UserId { get; set; }
        public string Predicate { get; set; }
    }
}