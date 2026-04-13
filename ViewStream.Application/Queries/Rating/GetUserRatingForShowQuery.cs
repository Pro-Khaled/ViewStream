using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Rating
{
    public record GetUserRatingForShowQuery(long ProfileId, long ShowId) : IRequest<RatingDto?>;

}
