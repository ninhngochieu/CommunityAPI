namespace BackendAPI.Modules
{
    public class UserParams
    {
        private int _pageSize = 10;
        private const int _maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
        }

        public string CurrentUsername { get; set; }
        
        public string Gender { get; set; }
    }
}