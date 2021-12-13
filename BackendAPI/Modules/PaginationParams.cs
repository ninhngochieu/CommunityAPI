namespace BackendAPI.Modules
{
    public class PaginationParams
    {
        protected int _pageSize = 10;
        protected const int _maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
        }
    }
}