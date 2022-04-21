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
using System.Text.Json;

namespace INK_API._Services.Services
{
    public class GluesService : IGluesService
    {
        private readonly IInkRepository _repoInk;
        private readonly IGluesRepository _repoGlues;
        private readonly IPartRepository _repoPart;
        private readonly ITreatmentWayRepository _repoTreatmentWay;
        private readonly IChemicalRepository _repoChemical;
        private readonly IPartInkChemicalRepository _repoPartInkChemical;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public GluesService(
            IPartInkChemicalRepository repoPartInkChemical, 
            IGluesRepository repoGlues,
            IPartRepository repoPart, 
            ITreatmentWayRepository repoTreatmentWay ,
            IChemicalRepository repoChemical, 
            IInkRepository repoInk, 
            IMapper mapper, 
            MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoGlues = repoGlues;
            _repoPart = repoPart;
            _repoTreatmentWay = repoTreatmentWay;
            _repoPartInkChemical = repoPartInkChemical;
            _repoInk = repoInk;
            _repoChemical = repoChemical;

        }

        public async Task<bool> UpdateGlueSequence(GlueSequenceDto update)
        {
            var item = _repoGlues.FindById(update.GlueDefaultID);
            item.Sequence = update.ToIndex;
            _repoGlues.Update(item);
            var items = _repoGlues.FindAll(x => x.ScheduleID == update.ScheduleID && x.ID != update.GlueDefaultID).ToList();

            foreach (var itemss in items)
            {
                if (itemss.Sequence <= update.ToIndex && itemss.Sequence >= update.FromIndex)
                {
                    itemss.Sequence = itemss.Sequence - 1;
                }
                if (itemss.Sequence <= update.FromIndex && itemss.Sequence >= update.ToIndex)
                {
                    itemss.Sequence = itemss.Sequence + 1;
                }
                
                //else
                //{
                //    itemss.Sequence = itemss.Sequence + 1;
                //}

            }
            _repoGlues.UpdateRange(items);
            //var itemChange = _repoGlues.FindAll(x => x.ScheduleID == update.ScheduleID && x.Sequence == update.ToIndex).FirstOrDefault();
            //if (itemChange != null)
            //{
            //    itemChange.Sequence = update.FromIndex;
            //    _repoGlues.Update(itemChange);
            //}
            return await _repoGlues.SaveAll();

            //throw new NotImplementedException();
        }

        public async Task<bool> Add(GluesDto model)
        {
            

            int index = 1;
            var item = _repoGlues.FindAll(x => x.ScheduleID == model.ScheduleID).Count();
            if (item == 0)
            {
                index = 1;

            } else
            {
                index = item + 1;
            }
            var glue = _mapper.Map<Glues>(model);
            glue.Sequence = index;
            _repoGlues.Add(glue);
            return await _repoGlues.SaveAll();

            //kiem tra PoGlue da ton tai chua - chua co thi them moi
            //var listPo_check = _repoPoGlues.FindAll(x => x.GlueID == glue.ID && x.ScheduleID == model.ScheduleID).FirstOrDefault();
            //if (listPo_check == null)
            //{
            //    //tim workplan ID
            //    var workplan_ID = _repoWorkPlan.FindAll(x => x.ScheduleID == model.ScheduleID).FirstOrDefault() != null
            //        ? _repoWorkPlan.FindAll(x => x.ScheduleID == model.ScheduleID).FirstOrDefault().ID : 0;
            //    var PoGlue = new PoGlue
            //    {
            //        ScheduleID = model.ScheduleID,
            //        GlueID = glue.ID,
            //        WorkPlanID = workplan_ID
            //    };
            //    _repoPoGlues.Add(PoGlue);
                
            //}
            //return await _repoPoGlues.SaveAll();
        }

        public Task<bool> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GluesDto>> GetAllAsync()
        {
            return await _repoGlues.FindAll().ProjectTo<GluesDto>(_configMapper).OrderByDescending(x => x.ID).ToListAsync();
        }

        public GluesDto GetById(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetGluesByScheduleID(int id)
        {
            var lists = await _repoGlues.FindAll(x => x.ScheduleID == id).OrderBy(x => x.Sequence).ToListAsync();
            var list = from a in lists
                       join b in _repoPart.FindAll() on a.PartID equals b.ID
                       join c in _repoTreatmentWay.FindAll() on a.TreatmentWayID equals c.ID
            select new GluesDto
            {
                ID = a.ID,
                Name = a.Name,
                PartID = a.PartID,
                TreatmentWayID = a.TreatmentWayID,
                CreatedDate = a.CreatedDate,
                ScheduleID = a.ScheduleID,
                Sequence = a.Sequence.ToInt(),
                Consumption = Math.Round(a.Consumption.ToDouble(), 2).ToString(),
                Part = b.Name,
                TreatmentWay = c.Name
            };
            return list.ToList();
        }


        public async Task<object> GetGluesByScheduleIDWithLocale(int id, string lang)
        {
            var lists = await _repoGlues.FindAll(x => x.ScheduleID == id).OrderBy(x => x.Sequence).ToListAsync();
            var list = from a in lists
                       join b in _repoPart.FindAll() on a.PartID equals b.ID
                       join c in _repoTreatmentWay.FindAll() on a.TreatmentWayID equals c.ID
                       select new GluesDto
                       {
                           ID = a.ID,
                           Name = a.Name,
                           PartID = a.PartID,
                           TreatmentWayID = a.TreatmentWayID,
                           CreatedDate = a.CreatedDate,
                           ScheduleID = a.ScheduleID,
                           Sequence = a.Sequence.ToInt(),
                           Consumption = Math.Round(a.Consumption.ToDouble(), 2).ToString(),
                           Part = b.Name,
                           TreatmentWay = lang == Constants.SystemLang.EN ? c.NameEn != null ? c.NameEn : c.Name : c.Name
                       };
            return list.ToList();
        }



        public async Task<bool> Deletes(int id, int scheduleID)
        {
            var item = _repoGlues.FindById(id);
            if (item.Sequence == null)
            {
                _repoGlues.Remove(item);
            }
            else
            {
                var sequenceItemDel = _repoGlues.FindById(id).Sequence;
                var items = _repoGlues.FindAll(x => x.ScheduleID == scheduleID && x.ID != id).ToList();

                foreach (var itemsss in items)
                {
                    if (itemsss.Sequence > sequenceItemDel)
                    {
                        itemsss.Sequence = itemsss.Sequence - 1;
                    }
                }
                _repoGlues.UpdateRange(items);
                _repoGlues.Remove(item);

            }
            return await _repoGlues.SaveAll();
        }

        public Task<PagedList<GluesDto>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<GluesDto>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(GluesDto model)
        {
            var glues = _mapper.Map<Glues>(model);
            _repoGlues.Update(glues);
            return await _repoGlues.SaveAll();
        }

        public async Task<object> SaveGlue(PartInkChemicalDto obj)
        {
            try
            {
                var glues = _repoGlues.FindById(obj.glueID);
                glues.Name = obj.name;
                var s1 = obj.listAdd.Where(x => x.subname == "Ink").FirstOrDefault() != null ? obj.listAdd.Where(x => x.subname == "Ink").FirstOrDefault().consumption : 0;
                var s2 = obj.listAdd.Where(x => x.subname == "Chemical").FirstOrDefault() != null ? obj.listAdd.Where(x => x.subname == "Chemical").Sum(x => x.consumption) : 0;
                var total = s1 + s2;
                glues.Consumption = total.ToString();
                _repoGlues.Update(glues);

                // xoa het partinkchemical - sau đó add lại
                var list_delete = _repoPartInkChemical.FindAll(x => x.GlueID == obj.glueID).ToList();
                _repoPartInkChemical.RemoveMultiple(list_delete);
                await _repoPartInkChemical.SaveAll();

                // kiem tra Po Glue - chua co thi them moi

                //var listPo_check = _repoPoGlues.FindAll(x => x.GlueID == obj.glueID && x.ScheduleID == obj.scheduleID).FirstOrDefault();
                //if (listPo_check == null)
                //{
                //    //tim workplan ID
                //    var workplan_ID = _repoWorkPlan.FindAll(x => x.ScheduleID == obj.scheduleID).FirstOrDefault() != null 
                //        ? _repoWorkPlan.FindAll(x => x.ScheduleID == obj.scheduleID).FirstOrDefault().ID : 0;
                //    var PoGlue = new PoGlue
                //    {
                //        ScheduleID = obj.scheduleID,
                //        GlueID = obj.glueID,
                //        WorkPlanID = workplan_ID
                //    };
                //    _repoPoGlues.Add(PoGlue);
                //    await _repoPoGlues.SaveAll();
                //}

                foreach (var item in obj.listAdd)
                {
                    if (item.subname == "Ink")
                    {
                        var listInk = obj.listAdd.Where(x => x.subname == "Ink").ToList();
                        var totals = obj.listAdd.Where(x => x.subname == "Ink").FirstOrDefault().consumption;
                        var unit = Math.Round(totals / listInk.Count, 2);
                        var result = _repoPartInkChemical.FindAll(x => x.PartID == obj.partID && x.InkID == item.ID && x.GlueID == obj.glueID).FirstOrDefault();
                        
                        // nếu khác Null thi update lai
                        if (result != null)
                        {
                            result.Percentage = item.percentage;
                            result.Consumption = unit;
                            _repoPartInkChemical.Update(result);
                            await _repoPartInkChemical.SaveAll();
                        }
                        else
                        {
                            //add new partInkChemical
                            var model = new PartInkChemical
                            {
                                PartID = obj.partID,
                                InkID = item.ID,
                                GlueID = obj.glueID,
                                Percentage = item.percentage,
                                Consumption = unit
                            };
                            //add new PoGlue
                            _repoPartInkChemical.Add(model);
                            await _repoPartInkChemical.SaveAll();
                        }
                    }
                    else
                    {
                        var result = _repoPartInkChemical.FindAll(x => x.PartID == obj.partID && x.ChemicalID == item.ID && x.GlueID == obj.glueID).FirstOrDefault();
                        // nếu khác Null thi update lai
                        if (result != null)
                        {
                            result.Percentage = item.percentage;
                            result.Consumption = item.consumption;
                            _repoPartInkChemical.Update(result);
                            await _repoPartInkChemical.SaveAll();
                        }
                        else
                        {
                            var model = new PartInkChemical
                            {
                                PartID = obj.partID,
                                ChemicalID = item.ID,
                                GlueID = obj.glueID,
                                Percentage = item.percentage,
                                Consumption = item.consumption
                            };
                            // var data = _repoPartInkChemical.FindAll();
                            _repoPartInkChemical.Add(model);
                            await _repoPartInkChemical.SaveAll();
                        }
                    }
                }
                // var message = "success";
                return new
                {
                    data = await _repoGlues.SaveAll(),
                    status = true,
                    message = "success"
                };
            }
            catch (System.Exception ex)
            {
                return new
                {
                    status = false,
                    message = "save to error"
                };
            }
        }

        public async Task<object> GetInkChemicalByGlueID(int id)
        {
            var list = new List<InkChemicalDto>();
            var data = await _repoPartInkChemical.FindAll(x => x.GlueID == id).ToListAsync();
            foreach (var item in data)
            {
                if (item.InkID != 0)
                {
                    var inkmodel = from a in await _repoInk.FindAll(x => x.ID == item.InkID).ToListAsync()
                                   from b in _repoPartInkChemical.FindAll(x => x.InkID == item.InkID && x.GlueID == id).DefaultIfEmpty()

                    select new InkChemicalDto
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Subname = "Ink",
                        modify = true,
                        Code = a.Code,
                        percentage = b.Percentage,
                        Consumption = b.Consumption != null ? b.Consumption : 0
                    };
                    list.AddRange(inkmodel);
                }
                else
                {
                    var chemicalmodel = from a in await _repoChemical.FindAll(x => x.ID == item.ChemicalID).ToListAsync()
                                        from b in _repoPartInkChemical.FindAll(x => x.ChemicalID == item.ChemicalID && x.GlueID == id).DefaultIfEmpty()

                    select new InkChemicalDto
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Code = a.Code,
                        Subname = "Chemical",
                        modify = a.Modify,
                        percentage = b.Percentage,
                        Consumption = b.Consumption != null ? b.Consumption : 0
                    };
                    list.AddRange(chemicalmodel);
                    
                }

            }

            //var total = list.Where(x => x.Subname == "Ink").Sum(y => y.Consumption).ToDouble();
            double total = 0;
            var total_glues = _repoGlues.FindById(id).Consumption;
            var list_chemical = _repoPartInkChemical.FindAll(x => x.GlueID == id && x.ChemicalID > 0).ToList();
            if (list_chemical.Count > 0)
            {
                double total_chemical = list_chemical.Sum(x => x.Consumption.ToDouble());
                total = total_glues.ToDouble() - total_chemical;
            } else
            {
                total = total_glues.ToDouble();
            }
            list.ForEach(item =>
            {
                if (item.Subname == "Ink")
                {
                    item.Consumption = Math.Round(total, 2);
                }
            });
            return list.OrderByDescending(x => x.Subname);
        }

        public async Task<object> GetInkChemicalByGlueIDWithLocale(int id, string lang)
        {
            var list = new List<InkChemicalDto>();
            var data = await _repoPartInkChemical.FindAll(x => x.GlueID == id).ToListAsync();
            foreach (var item in data)
            {
                if (item.InkID != 0)
                {
                    var inkmodel = from a in await _repoInk.FindAll(x => x.ID == item.InkID).ToListAsync()
                                   from b in _repoPartInkChemical.FindAll(x => x.InkID == item.InkID && x.GlueID == id).DefaultIfEmpty()

                                   select new InkChemicalDto
                                   {
                                       ID = a.ID,
                                       Name = lang == Constants.SystemLang.EN ? a.NameEn != null ? a.NameEn : a.Name : a.Name,
                                       Subname = "Ink",
                                       modify = true,
                                       Code = a.Code,
                                       percentage = b.Percentage,
                                       Consumption = b.Consumption != null ? b.Consumption : 0
                                   };
                    list.AddRange(inkmodel);
                }
                else
                {
                    var chemicalmodel = from a in await _repoChemical.FindAll(x => x.ID == item.ChemicalID).ToListAsync()
                                        from b in _repoPartInkChemical.FindAll(x => x.ChemicalID == item.ChemicalID && x.GlueID == id).DefaultIfEmpty()

                                        select new InkChemicalDto
                                        {
                                            ID = a.ID,
                                            Name = lang == Constants.SystemLang.EN ? a.NameEn != null ? a.NameEn : a.Name : a.Name,
                                            Code = a.Code,
                                            Subname = "Chemical",
                                            modify = a.Modify,
                                            percentage = b.Percentage,
                                            Consumption = b.Consumption != null ? b.Consumption : 0
                                        };
                    list.AddRange(chemicalmodel);

                }

            }

            //var total = list.Where(x => x.Subname == "Ink").Sum(y => y.Consumption).ToDouble();
            double total = 0;
            var total_glues = _repoGlues.FindById(id).Consumption;
            var list_chemical = _repoPartInkChemical.FindAll(x => x.GlueID == id && x.ChemicalID > 0).ToList();
            if (list_chemical.Count > 0)
            {
                double total_chemical = list_chemical.Sum(x => x.Consumption.ToDouble());
                total = total_glues.ToDouble() - total_chemical;
            }
            else
            {
                total = total_glues.ToDouble();
            }
            list.ForEach(item =>
            {
                if (item.Subname == "Ink")
                {
                    item.Consumption = Math.Round(total, 2);
                }
            });
            return list.OrderByDescending(x => x.Subname);
        }

    }
}