using System;
using System.Collections.Generic;

namespace ViewStream.Application.DTOs
{
    // These DTOs are referenced by ViewStream.API controllers but were missing in the Application layer.
    // Shapes are minimal so the project compiles; fill out fields as needed for your UI/clients.



    public class AdminItemVectorListItemDto
    {
        public long ShowId { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

}
