using System;
namespace demo_minimal_api
{
	public class UpsertArticletCommand
	{
        public string Title { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}

