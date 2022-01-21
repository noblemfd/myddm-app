using AutoMapper;
using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Core.Helpers;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Data.Application;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.DTOs;
using DDM.API.Infrastructure.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;

namespace DDM.API.Core.Services.v1.Concrete
{
    public class MerchantService : IMerchantService
    {
        //private IHttpContextAccessor _httpContextAccessor;
        public static int numberOfTimes = 0;
        private readonly DDMDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserResolverService _userResolverService;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<ApplicationRole> _roleManager;

        //  public MerchantService(IHttpContextAccessor httpContextAccessor, DDMDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        public MerchantService(DDMDbContext context, IMapper mapper, UserResolverService userResolverService, UserManager<ApplicationUser> userManager)
        {
          //  _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            _userResolverService = userResolverService;
            _userManager = userManager;
            //_roleManager = roleManager;
        }
        public async Task<GenericResponseDto<MandateListDto>> CreateMerchantMandateAsync(MandateCreateDto requestDto)
        {
            var userId = _userResolverService.GetUserName();
            var userId1 = _userResolverService.GetUserName();
            //var userId = long.Parse(userId1);
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userId).Select(m => m.Id).FirstOrDefault();
                //var existingMerchant = await _context.zib_merchants.FirstOrDefaultAsync(e => e.User.UserName == requestDto.UserName);
            //var response = new GenericResponseDto<AllMerchantListDto>();
            var response = new GenericResponseDto<MandateListDto>();

            //var existingUser = await _context.Departments.FirstOrDefaultAsync(d => d.Id == requestDto.DepartmentId);
            if (userId1 != null)
            {
                try
                {
                    var mandate = _mapper.Map<Mandate>(requestDto);
                   // string transactionJsonData = JsonSerializer.Serialize(mandate);
                    string transactionJsonData = JsonConvert.SerializeObject(mandate);
                    switch ((byte?)requestDto.PaymentFrequency)
                    {
                        case 1:
                            numberOfTimes = ConstantHelper.GetTotalMonth(mandate.StartDate, mandate.EndDate);
                            break;
                        case 2:
                            numberOfTimes = ConstantHelper.GetTotalQuarter(mandate.StartDate, mandate.EndDate);
                            break;
                        case 3:
                            numberOfTimes = ConstantHelper.GetTotalYear(mandate.StartDate, mandate.EndDate);
                            break;
                    }
                    var userName = _userResolverService.GetUserName();
                    mandate.RawData = transactionJsonData;
                    mandate.PaymentCount = (int)numberOfTimes;
                    mandate.MerchantId = merchantId;
                    mandate.CreatedBy = userName;
                    _context.zib_mandates.Add(mandate);
                    await _context.SaveChangesAsync();

                    var transaction = new TransactionLog();
                    transaction.MandateId = mandate.Id;
                    transaction.MerchantId = mandate.MerchantId;
                    transaction.RawData = mandate.RawData;
                    transaction.CreatedBy = userName;
                   // transaction.CreatedDate = DateTime.Now;

                    _context.zib_transaction_logs.Add(transaction);
                    await _context.SaveChangesAsync();

                    DateTime dueDate = mandate.StartDate;
                    decimal repaymentAmount = Convert.ToDecimal(mandate.Amount / mandate.PaymentCount);
                    for (int i = 1; i <= (int)mandate.PaymentCount; i++)
                    {
                        switch(mandate.PaymentFrequency)
                        {
                            case Infrastructure.Helpers.EnumList.PaymentFrequency.Monthly:
                            if(i != 1)
                            {
                                // Reset Date
                                dueDate = dueDate.AddMonths(1);
                            }
                            break;
                            case Infrastructure.Helpers.EnumList.PaymentFrequency.Quarterly:
                            if (i != 1)
                            {
                                // Reset Date
                                dueDate = dueDate.AddMonths(3);
                            }
                            break;
                            case Infrastructure.Helpers.EnumList.PaymentFrequency.Yearly:
                            if (i != 1)
                            {
                                // Reset Date
                                dueDate = dueDate.AddYears(1);
                            }
                            break;
                        }
                        var mandateDetail               = new MandateDetail();
                        mandateDetail.MandateId         = mandate.Id;
                        mandateDetail.MerchantId        = mandate.MerchantId;
                        mandateDetail.SerialNumber      = (i + 1) - 1;
                        mandateDetail.ReferenceNumber   = mandate.ReferenceNumber;
                        mandateDetail.DrAccountNumber   = mandate.DrAccountNumber;
                        mandateDetail.StartDate         = mandate.StartDate;
                        mandateDetail.PayableAmount     = repaymentAmount;
                        mandateDetail.EndDate           = mandate.EndDate;
                        mandateDetail.DueDate           = dueDate;
                        mandateDetail.CreatedBy         = userName;
                        //  mandateDetail.CreatedDate       = DateTime.Now;
                        _context.zib_mandate_details.Add(mandateDetail);
                        await _context.SaveChangesAsync();
                    }
                    response.StatusCode = 201;
                    response.Message = "Successfully Created Mandate and Schedules";
                    response.Result = _mapper.Map<MandateListDto>(mandate);
                }
                catch (Exception ex)
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 500,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 400,
                    Message = "Merchant not found!"
                };
                response.StatusCode = 400;
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetMandateAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId);
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateListDto>>(pagedMandates.ToList());
                    response.TotalPages = pagedMandates.PageCount;
                    response.Page = pagedMandates.PageNumber;
                    response.PerPage = pagedMandates.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateDetailListDto>> GetMandateDetailListAsync(long mandateId, int page, int limit)
        {
            var response = new PagedResponse<MandateDetailListDto>();
            var userName = _userResolverService.GetUserName();
           // var userId = long.Parse(userId1);
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandate_details.AsQueryable().Where(l => l.Mandate.Id == mandateId).Where(d => d.Mandate.MerchantId == merchantId);
                    var pagedMandateDetails = await mandateQueryable.ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateDetailListDto>>(pagedMandateDetails.ToList());
                    response.TotalPages = pagedMandateDetails.PageCount;
                    response.Page = pagedMandateDetails.PageNumber;
                    response.PerPage = pagedMandateDetails.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateWithDetailListDto>> GetMandateWithDetailListAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateWithDetailListDto>();
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => (int?)m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId);
                    var mandate = mandateQueryable.ToList();
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(m => m.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);
                  //  var mandateList = pagedMandates.ToList();
                  //  var stringRes = JsonConvert.SerializeObject(mandateList);

                  //  response.Result = _mapper.Map<List<MandateWithDetailListDto>>(mandateList);
                    response.Result = _mapper.Map<List<MandateWithDetailListDto>>(pagedMandates.ToList());
                    response.TotalPages = pagedMandates.PageCount;
                    response.Page = pagedMandates.PageNumber;
                    response.PerPage = pagedMandates.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetMandateByCutomerAsync(string custAccountNo, int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(l => l.DrAccountNumber == custAccountNo).Where(d => d.MerchantId == merchantId);
                    var pagedMandates = await mandateQueryable.ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateListDto>>(pagedMandates.ToList());
                    response.TotalPages = pagedMandates.PageCount;
                    response.Page = pagedMandates.PageNumber;
                    response.PerPage = pagedMandates.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> GetMandateByIdAsync(long id)
        {
            var response = new GenericResponseDto<MandateListDto>();

            var mandate = await _context.zib_mandates.Include(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (mandate != null)
            {
                response.Result = _mapper.Map<MandateListDto>(mandate);
                response.StatusCode = 200;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 404,
                    Message = "Mandate not found!"
                };
                response.StatusCode = 404;
            }

            return response;
        }
        public async Task<GenericResponseDto<MerchantProfileDto>> GetMerchantProfileAsync()
        {
            var response = new GenericResponseDto<MerchantProfileDto>();
            //var userId1 = _userResolverService.GetUserId();
            //var userId = long.Parse(userId1);
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();

            var merchantProfile = await _context.zib_merchants.Include(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == merchantId);

            if (merchantProfile != null)
            {
                response.Result = _mapper.Map<MerchantProfileDto>(merchantProfile);
                response.Message = "Successfully Retrieved Merchant Profile";
                response.StatusCode = 200;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 404,
                    Message = "Merchant not found!"
                };
                response.StatusCode = 404;

            }
            return response;
        }
        public async Task<GenericResponseDto<MerchantListDto>> GetMerchantByIdAsync(long id)
        {
            var response = new GenericResponseDto<MerchantListDto>();

            var merchant = await _context.zib_merchants.Include(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (merchant != null)
            {
                response.Result = _mapper.Map<MerchantListDto>(merchant);
                response.Message = "Successfully Retrieved Merchant";
                response.StatusCode = 200;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 404,
                    Message = "Merchant not found!"
                };
                response.StatusCode = 404;
            }

            return response;
        }
        public List<DashboardCountDto> GetDashboardFieldCount()
        {
            DashboardCountDto data = new DashboardCountDto();
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");

            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();

            data.AllMandateCount = _context.zib_mandates.Where(m => m.MerchantId == merchantId).Select(c => c.Id).Distinct().Count();
            data.CompletedPaymentCount = _context.zib_mandate_details.Where(m => m.MerchantId == merchantId).Where(m => (byte)m.MandateStatus == 2).Select(c => c.MandateId).Distinct().Count();
            data.ActiveCustomerCount = _context.zib_mandates.Where(m => m.MerchantId == merchantId).Select(c => c.DrAccountNumber).Distinct().Count();
            data.CurrentYearMandateCount = _context.zib_mandates.Where(m => m.CreatedDate >= currentYear).Where(m => m.MerchantId == merchantId).Select(c => c.Id).Distinct().Count();

            List<DashboardCountDto> dataCount = new List<DashboardCountDto>();

            dataCount.Add(data);

            return dataCount;
        }

        public async Task<PagedResponse<MandateWithDetailListDto>> GetCompletedPaymentListAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateWithDetailListDto>();
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => (int?)m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId && !m.MandateDetails.Any(md => (byte)md.MandateStatus != 2));
                    var mandate = mandateQueryable.ToList();
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(m => m.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);
                    response.Result = _mapper.Map<List<MandateWithDetailListDto>>(pagedMandates.ToList());
                    response.TotalPages = pagedMandates.PageCount;
                    response.Page = pagedMandates.PageNumber;
                    response.PerPage = pagedMandates.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetThisYearMandateAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.CreatedDate >= currentYear).Where(m => m.MerchantId == merchantId);
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateListDto>>(pagedMandates.ToList());
                    response.TotalPages = pagedMandates.PageCount;
                    response.Page = pagedMandates.PageNumber;
                    response.PerPage = pagedMandates.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetLatestMandateAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.Take(3).AsQueryable().OrderByDescending(c => c.CreatedDate).Where(m => m.MerchantId == merchantId);
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateListDto>>(pagedMandates.ToList());
                    response.TotalPages = pagedMandates.PageCount;
                    response.Page = pagedMandates.PageNumber;
                    response.PerPage = pagedMandates.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        //public async Task<MandateListDto> GetMonthlyMandateAsync()
        //{
        //    var response = new PagedResponse<MandateListDto>();
        //    DateTime current = DateTime.Now;
        //    DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");
        //    var userName = _userResolverService.GetUserName();
        //    var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
        //    try
        //    {
        //        {
        //            var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.CreatedDate >= currentYear).Where(m => m.MerchantId == merchantId);
        //            var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
        //                                        .ThenInclude(l => l.Merchant)
        //                                        .ThenInclude(e => e.User)
        //                                        .ToPagedListAsync(page, limit);

        //            response.Result = _mapper.Map<List<MandateListDto>>(pagedMandates.ToList());
        //            response.TotalPages = pagedMandates.PageCount;
        //            response.Page = pagedMandates.PageNumber;
        //            response.PerPage = pagedMandates.PageSize;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Error = new ErrorResponseDto()
        //        {
        //            ErrorCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //    return response;
        //}
    }
}
