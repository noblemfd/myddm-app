using AutoMapper;
using DDM.API.Core.DTOs.v1.Admin.Request;
using DDM.API.Core.DTOs.v1.Admin.Response;
using DDM.API.Core.Helpers;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Data.Application;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.DTOs;
using DDM.API.Infrastructure.Entities.Models;
using DDM.API.Infrastructure.Entities.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DDM.API.Core.Services.v1.Concrete
{
    public class AdminService : IAdminService
    {
        private readonly DDMDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserResolverService _userResolverService;

        public AdminService(DDMDbContext context, IMapper mapper, UserResolverService userResolverService, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _userResolverService = userResolverService;
        }
        public async Task<GenericResponseDto<AdminUserDto>> CreateAdminUserAsync(AdminCreateDto requestDto)
        {
            var response = new GenericResponseDto<AdminUserDto>();
            var existingUser = await _userManager.FindByNameAsync(requestDto.UserName);
            if (existingUser == null)
            {
                var user = _mapper.Map<ApplicationUser>(requestDto);
                user.SecurityStamp = Guid.NewGuid().ToString();
                var result = await _userManager.CreateAsync(user, requestDto.Password);

                if (!result.Succeeded)
                {
                    var error = string.Join<IdentityError>(", ", result.Errors.ToArray());
                    response.Error = new ErrorResponseDto { ErrorCode = 500, Message = "Failed to create user because of the following errors: " + error };
                }
                else
                {
                    response.StatusCode = 200;
                    response.Message = "Successfully Created Admmin User";
                    response.Result = _mapper.Map<AdminUserDto>(user);
                }
            }
            else
            {
                response.Error = new ErrorResponseDto { ErrorCode = 400, Message = "This Username is already registered!" };
                response.StatusCode = 400;
            }
            return response;
        }

        public async Task<GenericResponseDto<AllMerchantListDto>> CreateMerchantAsync(MerchantCreateDto requestDto)
        {
            var existingMerchant = await _context.zib_merchants.FirstOrDefaultAsync(e => e.User.UserName == requestDto.UserName);
            var response = new GenericResponseDto<AllMerchantListDto>();
            var userName = _userResolverService.GetUserName();

            if (existingMerchant != null)
            {
                response.Error = new ErrorResponseDto()
                {
                    ErrorCode = 400,
                    Message = "The Merchant's Username is already registered!"
                };
                response.StatusCode = 400;
            }
            else
            {
                var merchantUser = new ApplicationUser()
                {
                    MobileNumber = requestDto.MobileNumber,
                    UserName = requestDto.UserName,
                };

                var result = await _userManager.CreateAsync(merchantUser, "@SecretPassword123");

                if (result.Succeeded)
                {
                    var merchant = _mapper.Map<Merchant>(requestDto);
                    //merchant.UserId = merchantUser.Id;
                    //merchant.User = merchantUser;

                    // Assign Role
                    Task<IdentityResult> roleResult;
                    //Check if there is any Merchant Role; if not created it
                    Task<bool> hasMerchantRole = _roleManager.RoleExistsAsync(UserRoles.Merchant);
                    hasMerchantRole.Wait();
                    if (!hasMerchantRole.Result)
                    {
                        ApplicationRole roleCreate = new ApplicationRole();
                        roleCreate.Name = UserRoles.Merchant;
                        roleResult = _roleManager.CreateAsync(roleCreate);
                        roleResult.Wait();
                    }
                    Task<IdentityResult> newUserRole = _userManager.AddToRoleAsync(merchantUser, UserRoles.Merchant);
                    newUserRole.Wait();

                    try
                    {
                        // var userName = _userResolverService.GetUserName();
                        merchant.UserId = merchantUser.Id;
                        merchant.UserName = merchantUser.UserName;
                        merchant.CreatedBy = userName;
                        _context.zib_merchants.Add(merchant);
                        await _context.SaveChangesAsync();

                        response.Result = _mapper.Map<AllMerchantListDto>(merchant);
                        response.Message = "Successfully Created Merchant";
                        response.StatusCode = 201;
                    }
                    catch (Exception ex)
                    {
                        response.Error = new ErrorResponseDto()
                        {
                            ErrorCode = 500,
                            Message = ex.Message
                        };
                        response.StatusCode = 500;
                    }
                }
                else
                {
                    var error = "";
                    foreach (var identityError in result.Errors)
                    {
                        error += identityError.Description;
                    }

                    response.Error = new ErrorResponseDto { ErrorCode = 500, Message = "Failed to create Merchant because of the following errors: " + error };
                }
            }
            return response;
        }

        public async Task<PagedResponse<AllMerchantListDto>> GetMerchantAsync(int page, int limit)
        {
            var response = new PagedResponse<AllMerchantListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {

                    var merchantQueryable = _context.zib_merchants.Include(e => e.User).AsQueryable();
                    var pagedMerchants = await merchantQueryable.ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<AllMerchantListDto>>(pagedMerchants.ToList());
                    response.TotalPages = pagedMerchants.PageCount;
                    response.Page = pagedMerchants.PageNumber;
                    response.PerPage = pagedMerchants.PageSize;
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

        public async Task<GenericResponseDto<AllMerchantListDto>> GetMerchantByIdAsync(long id)
        {
            var response = new GenericResponseDto<AllMerchantListDto>();

            var merchant = await _context.zib_merchants.Include(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (merchant != null)
            {
                response.Result = _mapper.Map<AllMerchantListDto>(merchant);
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

        //public async Task<GenericResponseDto<AllMerchantListDto>> UpdateMerchantAsync(long id, MerchantCreateDto request)
        //{
        //    var response = new GenericResponseDto<AllMerchantListDto>();
        //    var merchant = await _context.zib_merchants.FirstOrDefaultAsync(s => s.Id == id);
        //    throw new NotImplementedException();
        //}

        public Task<GenericResponseDto<bool>> DeleteMerchantAsync(long id)
        {
            throw new NotImplementedException();
        }
        public async Task<PagedResponse<AllMandateListDto>> GetAllMandateAsync(int page, int limit)
        {
            var response = new PagedResponse<AllMandateListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable();
                    var pagedMandates = await mandateQueryable.Include(m => m.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<AllMandateListDto>>(pagedMandates.ToList());
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
        public async Task<PagedResponse<AllMandateWithDetailListDto>> GetMandateWithDetailListAsync(int page, int limit)
        {
            var response = new PagedResponse<AllMandateWithDetailListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable();
                    var mandate = mandateQueryable.ToList();
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(m => m.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);
                    response.Result = _mapper.Map<List<AllMandateWithDetailListDto>>(pagedMandates.ToList());
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
        public async Task<PagedResponse<AllMerchantListDto>> GetAllMandateByMerchantAsync(long merchantId, int page, int limit)
        {
            var response = new PagedResponse<AllMerchantListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.Where(l => l.Merchant.Id == merchantId).AsQueryable();
                    var pagedMandates = await mandateQueryable.ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<AllMerchantListDto>>(pagedMandates.ToList());
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
        public async Task<GenericResponseDto<AllMandateListDto>> GetMandateByIdAsync(long id)
        {
            var response = new GenericResponseDto<AllMandateListDto>();

            var mandate = await _context.zib_mandates.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (mandate != null)
            {
                response.Result = _mapper.Map<AllMandateListDto>(mandate);
                response.Message = "Successfully Retrieved Mandate";
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
        public async Task<PagedResponse<AllMandateDetailListDto>> GetAllMandateDetailListAsync(long mandateId, int page, int limit)
        {
            var response = new PagedResponse<AllMandateDetailListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandate_details.Where(l => l.Mandate.Id == mandateId).AsQueryable();
                    var pagedMandateDetails = await mandateQueryable.ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<AllMandateDetailListDto>>(pagedMandateDetails.ToList());
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
        //public AdminDashboardDto GetAdminDashboard(string userName)
        //{
        //    var totalMandateCount = 0;
        //    var completedPaymentCount = 0;
        //    var activeMerchantCount = 0;
        //    var currentYearMandateCount = 0;

        //    AdminDashboardDto data = new AdminDashboardDto();

        //    return data;
        //}
        public List<AdminDashboardCountDto> GetDashboardFieldCount()
        {
            AdminDashboardCountDto data = new AdminDashboardCountDto();
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");

            data.AllMandateCount = _context.zib_mandates.Select(c => c.Id).Distinct().Count();
            data.CompletedPaymentCount = _context.zib_mandate_details.Where(m => (byte)m.MandateStatus == 2).Select(c => c.MandateId).Distinct().Count();
            data.ActiveMerchantCount = _context.zib_merchants.Select(c => c.MerchantName).Distinct().Count();
            data.CurrentYearMandateCount = _context.zib_mandates.Where(m => m.CreatedDate >= currentYear).Select(c => c.Id).Distinct().Count();

            List<AdminDashboardCountDto> dataCount = new List<AdminDashboardCountDto>();

            dataCount.Add(data);

            return dataCount;
        }

        public async Task<PagedResponse<AllMandateWithDetailListDto>> GetCompletedPaymentListAsync(int page, int limit)
        {
            var response = new PagedResponse<AllMandateWithDetailListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => !m.MandateDetails.Any(md => (byte)md.MandateStatus != 2));
                    var mandate = mandateQueryable.ToList();
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(m => m.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);
                    response.Result = _mapper.Map<List<AllMandateWithDetailListDto>>(pagedMandates.ToList());
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
        public async Task<PagedResponse<AllMandateListDto>> GetThisYearMandateAsync(int page, int limit)
        {
            var response = new PagedResponse<AllMandateListDto>();
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.CreatedDate >= currentYear);
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<AllMandateListDto>>(pagedMandates.ToList());
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
        public async Task<PagedResponse<AllMandateListDto>> GetLatestMandateAsync(int page, int limit)
        {
            var response = new PagedResponse<AllMandateListDto>();
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.Take(5).AsQueryable().OrderByDescending(c => c.CreatedDate);
                    var pagedMandates = await mandateQueryable.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<AllMandateListDto>>(pagedMandates.ToList());
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
        public List<AdminMonthlySumDto> GetMandateMonthlySum()
        {
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");

            var monthlyMandate = _context.zib_mandates.Where(m => m.CreatedDate >= currentYear)
                .GroupBy(o => new
                {
                    Month = o.CreatedDate.Value.Month
                })
                .Select(u => new AdminMonthlySumDto
                {
                    ItemSum = u.Sum(x => x.Amount),
                    Month = u.Key.Month,
                    MonthName = u.Key.Month.ToString("MMMM")
                    //MonthName = u.Select(m => m.CreatedDate.Value.ToString("MMMM")).FirstOrDefault()
                    //  MonthName = DateTime.ParseExact(MonthNameValue, "MMMM", CultureInfo.CurrentCulture).Month
                })
                .ToList();
            // monthlyMandate.ForEach(var userdata in monthlyMandate)
          //  DateTime dt = DateTime.Now;
          //  Console.WriteLine(dt.ToString("MMMM"));
         // .Select(m => m.Id).FirstOrDefault();
            return monthlyMandate;
        }

        public List<AdminYearlySumDto> GetFiveYearMandate()
        {
            var yearlyMandate = _context.zib_mandates.Where(m => m.CreatedDate > DateTime.Now.AddYears(-5))
                .GroupBy(o => o.CreatedDate.Value.Year)
                .Select(u => new AdminYearlySumDto
                {
                    ItemTotal = u.Sum(x => x.Amount),
                    Year = u.Key 
        }
                ).ToList();

            //grand total
            var tot = yearlyMandate.Sum(s => s.ItemTotal);

            //apply percentage to each element
            yearlyMandate.ForEach(s => s.ItemPercent = (int)((decimal)100.0 * s.ItemTotal / tot));

            return yearlyMandate;
        }
    }
}
