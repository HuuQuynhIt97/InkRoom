using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INK_API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using INK_API._Repositories.Interface;
using INK_API._Services.Interface;
using INK_API.DTO;
using INK_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace INK_API._Services.Services
{
    public class PoGlueService : IPoGlueService
    {
        private readonly IPoGlueRepository _repo;

        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public PoGlueService(
            IPoGlueRepository repo,
            IMapper mapper,
            MapperConfiguration configMapper)
        {
            
            _configMapper = configMapper;
            _repo = repo;
            _mapper = mapper;
           

        }

        
    }
}