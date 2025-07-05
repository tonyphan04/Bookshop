namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Generic API response wrapper for consistent response format across all endpoints.
    /// Provides standardized success/error handling, messaging, and error collection.
    /// Makes frontend error handling predictable and easier to implement.
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Human-readable message describing the result
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The actual data payload (null if operation failed)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Collection of error messages for validation or processing errors
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Creates a successful API response with data and optional message.
        /// Used when operations complete successfully and return data.
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">Success message (default: "Operation successful")</param>
        /// <returns>ApiResponse indicating success with data</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates an error API response with message and optional error details.
        /// Used when operations fail due to validation, business rules, or system errors.
        /// </summary>
        /// <param name="message">Error message describing what went wrong</param>
        /// <param name="errors">Detailed error messages (e.g., validation errors)</param>
        /// <returns>ApiResponse indicating failure with error details</returns>
        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    /// <summary>
    /// Wrapper for paginated data responses with metadata about pagination state.
    /// Provides all information needed for implementing pagination UI components.
    /// Calculates pagination metadata automatically to reduce frontend complexity.
    /// </summary>
    /// <typeparam name="T">The type of data being paginated</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The data items for the current page
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages available
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Whether there are more pages after the current page
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Whether there are pages before the current page
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Creates a paginated response with automatic calculation of pagination metadata.
        /// Eliminates the need to manually calculate pagination state in controllers.
        /// </summary>
        /// <param name="data">The data items for this page</param>
        /// <param name="currentPage">Current page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="totalCount">Total items across all pages</param>
        public PaginatedResponse(List<T> data, int currentPage, int pageSize, int totalCount)
        {
            Data = data;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasNextPage = currentPage < TotalPages;
            HasPreviousPage = currentPage > 1;
        }
    }

    /// <summary>
    /// Base search request with common pagination and sorting parameters.
    /// Provides standard search functionality that can be extended for specific entities.
    /// Reduces code duplication across different search endpoints.
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Search query string for text-based filtering
        /// </summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// Page number for pagination (1-based, default: 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 10)
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Property name to sort by (default: "Id")
        /// </summary>
        public string SortBy { get; set; } = "Id";

        /// <summary>
        /// Whether to sort in descending order (default: false for ascending)
        /// </summary>
        public bool SortDescending { get; set; } = false;
    }

    /// <summary>
    /// Extended search request specifically for book searches with additional filters.
    /// Inherits common search functionality and adds book-specific filtering options.
    /// Allows complex book searches with multiple criteria for better user experience.
    /// </summary>
    public class BookSearchRequest : SearchRequest
    {
        /// <summary>
        /// Filter books by category (optional)
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Minimum price filter for price range searches (optional)
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum price filter for price range searches (optional)
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Filter to show only books that are in stock (optional)
        /// </summary>
        public bool? InStockOnly { get; set; }
    }
}
