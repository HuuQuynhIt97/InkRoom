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
using Microsoft.AspNetCore.Http;

namespace INK_API._Services.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repoSupplier;
        private readonly IProcessRepository _repoProcess;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IHttpContextAccessor _accessor;

        public SupplierService(IProcessRepository repoProcess ,ISupplierRepository repoSupplier,IHttpContextAccessor accessor, IMapper mapper, MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoSupplier = repoSupplier;
            _accessor = accessor;
            _repoProcess = repoProcess;

        }

        //Thêm Supplier mới vào bảng Supplier
        public async Task<bool> Add(SuppilerDto model)
        {
            var Supplier = _mapper.Map<Supplier>(model);
            Supplier.isShow = true;
            Supplier.ProcessID = model.ProcessID;
            _repoSupplier.Add(Supplier);
            return await _repoSupplier.SaveAll();
        }


        //Lấy danh sách Supplier và phân trang
        public async Task<PagedList<SuppilerDto>> GetWithPaginations(PaginationParams param)
        {
            var lists = _repoSupplier.FindAll(x => x.isShow == true).ProjectTo<SuppilerDto>(_configMapper).OrderBy(x => x.ID);
            return await PagedList<SuppilerDto>.CreateAsync(lists, param.PageNumber, param.PageSize);
        }
      
        //Tìm kiếm Supplier
        public async Task<PagedList<SuppilerDto>> Search(PaginationParams param, object text)
        {
            var lists = _repoSupplier.FindAll(x => x.isShow == true).ProjectTo<SuppilerDto>(_configMapper)
            .Where(x => x.Name.Contains(text.ToString()))
            .OrderBy(x => x.ID);
            return await PagedList<SuppilerDto>.CreateAsync(lists, param.PageNumber, param.PageSize);
        }

        //Xóa Supplier
        public async Task<bool> Delete(object id)
        {
            string token = _accessor.HttpContext.Request.Headers["Authorization"];
            var userID = JWTExtensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            if (userID == 0) return false;
            var supplier = _repoSupplier.FindById(id);
            _repoSupplier.Remove(supplier);
            // supplier.isShow = false;
            // supplier.ModifiedBy = userID;
            // supplier.ModifiedDate = DateTime.Now;
            return await _repoSupplier.SaveAll();
        }

        //Cập nhật Supplier
        public async Task<bool> Update(SuppilerDto model)
        {
            string token = _accessor.HttpContext.Request.Headers["Authorization"];
            var userID = JWTExtensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            if (userID == 0) return false;
            var supplier = _mapper.Map<Supplier>(model);
            supplier.isShow = true;
            supplier.ProcessID = model.ProcessID;
            supplier.ModifiedBy = userID;
            supplier.ModifiedDate = DateTime.Now;
            _repoSupplier.Update(supplier);
            return await _repoSupplier.SaveAll();
        }
      
        //Lấy toàn bộ danh sách Supplier 
        public async Task<List<SuppilerDto>> GetAllAsync()
        {
            var data = new List<SuppilerDto>();
            var model = await _repoSupplier.FindAll().ToListAsync();
            data = model.Select(x => new SuppilerDto
            {
                ID = x.ID,
                Name = x.Name,
                ProcessID = x.ProcessID,
                Process = _repoProcess.FindById(x.ProcessID) != null ? _repoProcess.FindById(x.ProcessID).Name : null
            }).OrderByDescending(x => x.ID).ToList();
            return data;
        }

        //Lấy Supplier theo Supplier_Id
        public SuppilerDto GetById(object id)
        {
            return  _mapper.Map<Supplier, SuppilerDto>(_repoSupplier.FindById(id));
        }

        public async Task<object> GetAllWithName()
        {
            return await _repoSupplier.FindAll(x => x.isShow == true).Select(x => new SuppilerDto {
                ID = x.ID,
                Name = x.Name + " - " + _repoProcess.FindAll(y => y.ID == x.ProcessID).FirstOrDefault().Name,
                Process = _repoProcess.FindAll(y => y.ID == x.ProcessID).FirstOrDefault().Name
            }).OrderByDescending(x => x.ID).ToListAsync();
        }

        public async Task<object> GetAllByTreatment(int id)
        {
            var data = from a in  _repoSupplier.FindAll(x => x.isShow == true && x.ProcessID == id)
                       join b in _repoProcess.FindAll() on a.ProcessID equals b.ID
                       select new SuppilerDto
                       {
                           ID = a.ID,
                           Name = a.Name,
                           Process = b.Name
                       };

            return data.OrderByDescending(x => x.ID).ToList();
        }
    }
}