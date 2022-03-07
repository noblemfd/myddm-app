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
using System.Threading.Tasks;
using X.PagedList;
using System.Globalization;
using DDM.API.Infrastructure.Entities.Roles;

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
        //private readonly PasswordHasher<ApplicationUser> _passwordHasher;
        private readonly RoleManager<ApplicationRole> _roleManager;
        //public MerchantService(DDMDbContext context, IMapper mapper, UserResolverService userResolverService, UserManager<ApplicationUser> userManager, PasswordHasher<ApplicationUser> passwordHasher, RoleManager<ApplicationRole> roleManager)
        public MerchantService(DDMDbContext context, IMapper mapper, UserResolverService userResolverService, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
          //  _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            _userResolverService = userResolverService;
            _userManager = userManager;
            //_passwordHasher = passwordHasher;
            _roleManager = roleManager;
        }
        public async Task<GenericResponseDto<MerchantUserListDto>> CreateMerchantUserAsync(MerchantUserCreateDto requestDto)
        {
            var userName = _userResolverService.GetUserName();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userId).Select(m => m.Id).FirstOrDefault();
            var existingMerchant = await _context.zib_merchant_users.FirstOrDefaultAsync(e => e.User.UserName == requestDto.UserName || e.User.MobileNumber == requestDto.MobileNumber);
            var response = new GenericResponseDto<MerchantUserListDto>();

            if (existingMerchant != null)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 400,
                    Message = "The User is already registered!"
                };
                response.StatusCode = 400;
            }
            else
            {
                var merchantUser = new ApplicationUser()
                {
                    MobileNumber    = requestDto.MobileNumber,
                    UserName        = requestDto.UserName,
                    FirstName       = requestDto.FirstName,
                    LastName        = requestDto.LastName,
                    Email           = requestDto.Email
                };
                var result = await _userManager.CreateAsync(merchantUser, requestDto.Password);

                if (result.Succeeded)
                {
                    var merchant = _mapper.Map<MerchantUser>(requestDto);
                    // Assign Role
                    Task<IdentityResult> roleResult;
                    //Check if there is any MerchantUser Role; if not created it
                    Task<bool> hasMerchantRole = _roleManager.RoleExistsAsync(UserRoles.MerchantUser);
                    hasMerchantRole.Wait();
                    if (!hasMerchantRole.Result)
                    {
                        ApplicationRole roleCreate = new ApplicationRole();
                        roleCreate.Name = UserRoles.MerchantUser;
                        roleResult = _roleManager.CreateAsync(roleCreate);
                        roleResult.Wait();
                    }
                    Task<IdentityResult> newUserRole = _userManager.AddToRoleAsync(merchantUser, UserRoles.MerchantUser);
                    newUserRole.Wait();
                    try
                    {
                        // var userName = _userResolverService.GetUserName();
                        merchant.UserId = merchantUser.Id;
                        if (loggedUserRoleName == "Merchant")
                        {
                            merchant.MerchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
                        }
                        else
                        {
                            merchant.MerchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
                        }
                      //  merchant.MerchantId = merchantId;
                        merchant.IsMerchantAdmin = requestDto.IsMerchantAdmin;
                        merchant.CreatedBy = userName;
                        _context.zib_merchant_users.Add(merchant);
                        await _context.SaveChangesAsync();

                        response.Result = _mapper.Map<MerchantUserListDto>(merchant);
                        response.Message = "Successfully Created Merchant User";
                        response.StatusCode = 200;
                        response.Success = true;
                    }
                    catch (Exception ex)
                    {
                        response.Error = new ErrorResponseDto()
                        {
                            Success = false,
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
                    response.Error = new ErrorResponseDto { Success = false, ErrorCode = 500, Message = "Failed to create Merchant because of the following errors: " + error };
                }
            }
            return response;
        }
        public async Task<PagedResponse<MerchantUserListDto>> GetMerchantUserAsync(int page, int limit)
        {
            var response = new PagedResponse<MerchantUserListDto>();
            var userName = _userResolverService.GetUserName();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }

            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var merchantQueryable = _context.zib_merchant_users.Include(e => e.User).Include(l => l.Merchant).AsQueryable();
                    var pagedMerchants = await merchantQueryable.ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MerchantUserListDto>>(pagedMerchants.ToList());
                    response.TotalPages = pagedMerchants.PageCount;
                    response.Page = pagedMerchants.PageNumber;
                    response.PerPage = pagedMerchants.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }

            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }

            return response;
        }
        public async Task<GenericResponseDto<MerchantUserListDto>> GetMerchantUserByIdAsync(long id)
        {
            var response = new GenericResponseDto<MerchantUserListDto>();

            var userName = _userResolverService.GetUserName();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }

            var merchant = await _context.zib_merchant_users.Include(e => e.User).Include(l => l.Merchant)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (merchant != null)
            {
                response.Result = _mapper.Map<MerchantUserListDto>(merchant);
                response.Message = "Successfully Retrieved Merchant User";
                response.StatusCode = 200;
                response.Success = true;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Merchant User not found!"
                };
                response.StatusCode = 404;
            }

            return response;
        }
        public async Task<GenericResponseDto<MerchantUserListDto>> UpdateMerchantUserAsync(long id, MerchantUserUpdateDto requestDto)
        {
            var response = new GenericResponseDto<MerchantUserListDto>();

            var merchantUser = await _context.zib_merchant_users.FirstOrDefaultAsync(s => s.Id == id);
            var userId = _context.zib_merchant_users.Where(u => u.Id == id).Select(m => m.UserId).FirstOrDefault();
            var userId1 = userId.ToString();
            var userName = _userResolverService.GetUserName();

            if (merchantUser != null)
            {
                try
                {
                    // Get the existing merchant user from the db
                    var appUser = await _userManager.FindByIdAsync(userId1);
                    if (appUser != null)
                    {
                        appUser.MobileNumber = requestDto.MobileNumber;
                        appUser.UserName = requestDto.UserName;
                        appUser.FirstName = requestDto.FirstName;
                        appUser.LastName = requestDto.LastName;
                        appUser.Email = requestDto.Email;
                        //appUser.PasswordHash = requestDto.Password//checkUser.PasswordHash;
                        //if (!string.IsNullOrEmpty(requestDto.Password))
                        //{
                        //    appUser.PasswordHash = _passwordHasher.HashPassword(appUser, requestDto.Password);
                        //}
                        //else
                        //{
                        //    response.Error = new ErrorResponseDto()
                        //    {
                        //        ErrorCode = 400,
                        //        Message = "Password cannot be empty!"
                        //    };
                        //    response.StatusCode = 400;
                        //}
                    }
                    var result = await _userManager.UpdateAsync(appUser);
                    if (result.Succeeded)
                    {
                        var updatedMerchantUser = _mapper.Map(requestDto, merchantUser);
                        updatedMerchantUser.IsMerchantAdmin = requestDto.IsMerchantAdmin;
                        updatedMerchantUser.LastUpdatedBy = userName;
                        _context.zib_merchant_users.Add(updatedMerchantUser);
                        await _context.SaveChangesAsync();

                        response.Result = _mapper.Map<MerchantUserListDto>(updatedMerchantUser);
                        response.Message = "Successfully Updated Merchant User";
                        response.StatusCode = 200;
                        response.Success = true;
                    }
                    else
                    {
                        var error = "";
                        foreach (var identityError in result.Errors)
                        {
                            error += identityError.Description;
                        }
                        response.Error = new ErrorResponseDto { Success = false, ErrorCode = 500, Message = "Failed to update Merchant User because of the following errors: " + error };
                    }
                }
                catch (Exception ex)
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 500,
                        Message = ex.Message
                    };
                    response.StatusCode = 500;
                }
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Merchant User not found!"
                };
                response.StatusCode = 404;
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> CancelMandateAsync(long id, MandateCancelDto requestDto)
        {
            var response = new GenericResponseDto<MandateListDto>();
            var mandate = await _context.zib_mandates.FirstOrDefaultAsync(m => m.Id == id);
            var userName = _userResolverService.GetUserName();

            if (mandate != null)
            {
                try
                {
                    mandate.IsCancelled = true;
                    mandate.CancellationNote = requestDto.CancellationNote;
                    mandate.CancellationBy = userName;
                    mandate.CancellationDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    response.Result = _mapper.Map<MandateListDto>(mandate);
                    response.Message = "Successfully Cancelled Mandate";
                    response.StatusCode = 200;
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 500,
                        Message = ex.Message
                    };
                    response.StatusCode = 500;
                }
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Mandate not found!"
                };
                response.StatusCode = 404;
            }

            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> CancelMandateByCustomerRefAsync(string custAccountNo, string mandateRefNo, MandateCancelDto requestDto)
        {
            var response = new GenericResponseDto<MandateListDto>();
            var mandate = await _context.zib_mandates.FirstOrDefaultAsync(m => m.DrAccountNumber == custAccountNo && m.ReferenceNumber == mandateRefNo);
            var userName = _userResolverService.GetUserName();

            if (mandate != null)
            {
                try
                {
                    mandate.IsCancelled = true;
                    mandate.CancellationNote = requestDto.CancellationNote;
                    mandate.CancellationBy = userName;
                    mandate.CancellationDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    response.Result = _mapper.Map<MandateListDto>(mandate);
                    response.Message = "Successfully Cancelled Mandate";
                    response.StatusCode = 200;
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 500,
                        Message = ex.Message
                    };
                    response.StatusCode = 500;
                }
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Mandate not found!"
                };
                response.StatusCode = 404;
            }

            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetMandateCancelledAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId).Where(m => (bool)m.IsCancelled);
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetMandateCancelledByCustomerAsync(string custAccountNo, int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId).Where(m => m.DrAccountNumber == custAccountNo && (bool)m.IsCancelled);
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> GetMandateCancelledByCustomerRefAsync(string custAccountNo, string mandateRefNo)
        {
            var response = new GenericResponseDto<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                var custMandate = await _context.zib_mandates.Include(m => m.Merchant).ThenInclude(m => m.User).FirstOrDefaultAsync(m => m.MerchantId == merchantId && m.DrAccountNumber == custAccountNo && m.ReferenceNumber == mandateRefNo && (bool)m.IsCancelled);
                if (custMandate != null)
                {
                    response.Result = _mapper.Map<MandateListDto>(custMandate);
                    response.Message = "Successfully Retrieved Mandate";
                    response.StatusCode = 200;
                    response.Success = true;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 404,
                        Message = "Mandate not found!"
                    };
                    response.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> CreateMerchantMandateAsync(MandateCreateDto requestDto)
        {
            var userId = _userResolverService.GetUserName();
            var userId1 = _userResolverService.GetUserName();
            var getUser = await _userManager.FindByNameAsync(userId);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();

            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userId).Select(m => m.Id).FirstOrDefault();
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
                            numberOfTimes = ConstantHelper.GetTotalBiAnnual(mandate.StartDate, mandate.EndDate);
                            break;
                        case 4:
                            numberOfTimes = ConstantHelper.GetTotalYear(mandate.StartDate, mandate.EndDate);
                            break;
                    }
                    var userName = _userResolverService.GetUserName();
                    mandate.RawData = transactionJsonData;
                    mandate.PaymentCount = (int)numberOfTimes;
                    // mandate.MerchantId = merchantId;
                    if (loggedUserRoleName == "Merchant")
                    {
                        mandate.MerchantId = _context.zib_merchants.Where(u => u.UserName == userId).Select(m => m.Id).FirstOrDefault();
                    }
                    else
                    {
                        mandate.MerchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userId).Select(m => m.Id).FirstOrDefault();
                    }
                    mandate.CreatedBy = userName;
                    _context.zib_mandates.Add(mandate);
                    await _context.SaveChangesAsync();

                    var transaction = new TransactionLog();
                    transaction.MandateId = mandate.Id;
                    transaction.MerchantId = mandate.MerchantId;
                    transaction.RawData = mandate.RawData;
                    transaction.CreatedBy = userName;
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
                            case Infrastructure.Helpers.EnumList.PaymentFrequency.BiAnnual:
                            if (i != 1)
                            {
                                // Reset Date
                                dueDate = dueDate.AddMonths(6);
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
                        _context.zib_mandate_details.Add(mandateDetail);
                        await _context.SaveChangesAsync();
                    }
                    response.StatusCode = 201;
                    response.Success = true;
                    response.Message = "Successfully Created Mandate and Schedules";
                    response.Result = _mapper.Map<MandateListDto>(mandate);
            }
                catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
        }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
           // var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
          //  var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }

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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
           // var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => (int?)m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetMandateApprovedAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId).Where(m => (bool)m.IsApproved);
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateListDto>> GetMandateApprovedByCustomerAsync(string custAccountNo, int page, int limit)
        {
            var response = new PagedResponse<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandates.AsQueryable().Where(m => m.MerchantId == merchantId).Where(m => m.DrAccountNumber == custAccountNo && (bool)m.IsApproved);
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> GetMandateApprovedByCustomerRefAsync(string custAccountNo, string mandateRefNo)
        {
            var response = new GenericResponseDto<MandateListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                var custMandate = await _context.zib_mandates.Include(m => m.Merchant).ThenInclude(m => m.User).FirstOrDefaultAsync(m => m.MerchantId == merchantId && m.DrAccountNumber == custAccountNo && m.ReferenceNumber == mandateRefNo && (bool)m.IsApproved);
                if (custMandate != null)
                {
                    response.Result = _mapper.Map<MandateListDto>(custMandate);
                    response.Message = "Successfully Retrieved Mandate";
                    response.StatusCode = 200;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 404,
                        Message = "Mandate not found!"
                    };
                    response.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateDetailListDto>> GetMandatePaymentAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateDetailListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandate_details.AsQueryable().Where(d => d.Mandate.MerchantId == merchantId && (byte)d.MandateStatus == 2);
                    var pagedMandateDetails = await mandateQueryable.Include(l => l.Mandate)
                                                .ThenInclude(m => m.Merchant)
                                                .ThenInclude(e => e.User).ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateDetailListDto>>(pagedMandateDetails.ToList());
                    response.TotalPages = pagedMandateDetails.PageCount;
                    response.Page = pagedMandateDetails.PageNumber;
                    response.PerPage = pagedMandateDetails.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<PagedResponse<MandateDetailListDto>> GetMandatePaymentByCutomerAsync(string custAccountNo, int page, int limit)
        {
            var response = new PagedResponse<MandateDetailListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                if (page >= 1 && limit >= 1)
                {
                    var mandateQueryable = _context.zib_mandate_details.AsQueryable().Where(d => d.Mandate.MerchantId == merchantId && d.DrAccountNumber == custAccountNo && (byte)d.MandateStatus == 2);
                    var pagedMandateDetails = await mandateQueryable.Include(l => l.Mandate)
                                                .ThenInclude(m => m.Merchant)
                                                .ThenInclude(e => e.User).ToPagedListAsync(page, limit);

                    response.Result = _mapper.Map<List<MandateDetailListDto>>(pagedMandateDetails.ToList());
                    response.TotalPages = pagedMandateDetails.PageCount;
                    response.Page = pagedMandateDetails.PageNumber;
                    response.PerPage = pagedMandateDetails.PageSize;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateDetailListDto>> GetMandatePaymentByCustomerRefAsync(string custAccountNo, string mandateRefNo)
        {
            var response = new GenericResponseDto<MandateDetailListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            try
            {
                var custMandate = await _context.zib_mandate_details.Include(m => m.Merchant).ThenInclude(m => m.User).FirstOrDefaultAsync(m => m.MerchantId == merchantId && m.DrAccountNumber == custAccountNo && m.ReferenceNumber == mandateRefNo && (byte)m.MandateStatus == 2);
                if (custMandate != null)
                {
                    response.Result = _mapper.Map<MandateDetailListDto>(custMandate);
                    response.Message = "Successfully Retrieved Mandate";
                    response.StatusCode = 200;
                }
                else
                {
                    response.Error = new ErrorResponseDto()
                    {
                        Success = false,
                        ErrorCode = 404,
                        Message = "Mandate not found!"
                    };
                    response.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<GenericResponseDto<MandateListDto>> GetMandateByIdAsync(long id)
        {
            var response = new GenericResponseDto<MandateListDto>();
            var mandate = await _context.zib_mandates.Include(l => l.MandateDetails)
                                                .ThenInclude(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == id);
            if (mandate != null)
            {
                response.Result = _mapper.Map<MandateListDto>(mandate);
                response.StatusCode = 200;
                response.Success = true;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }

            var merchantProfile = await _context.zib_merchants.Include(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == merchantId);

            if (merchantProfile != null)
            {
                response.Result = _mapper.Map<MerchantProfileDto>(merchantProfile);
                response.Message = "Successfully Retrieved Merchant Profile";
                response.StatusCode = 200;
                response.Success = true;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
                response.Success = true;
            }
            else
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Merchant not found!"
                };
                response.StatusCode = 404;
            }

            return response;
        }
        public async Task<List<MerchantDashboardCountDto>> GetDashboardFieldCount()
        {
            MerchantDashboardCountDto data = new MerchantDashboardCountDto();
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");

            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }

            data.AllMandateCount = _context.zib_mandates.Where(m => m.MerchantId == merchantId).Select(c => c.Id).Distinct().Count();
            data.CompletedPaymentCount = _context.zib_mandate_details.Where(m => m.MerchantId == merchantId).Where(m => (byte)m.MandateStatus == 2).Select(c => c.MandateId).Distinct().Count();
            data.ActiveCustomerCount = _context.zib_mandates.Where(m => m.MerchantId == merchantId).Select(c => c.DrAccountNumber).Distinct().Count();
            data.CurrentYearMandateCount = _context.zib_mandates.Where(m => m.CreatedDate >= currentYear).Where(m => m.MerchantId == merchantId).Select(c => c.Id).Distinct().Count();

            List<MerchantDashboardCountDto> dataCount = new List<MerchantDashboardCountDto>();

            dataCount.Add(data);

            return dataCount;
        }
        public async Task<PagedResponse<MandateWithDetailListDto>> GetCompletedPaymentListAsync(int page, int limit)
        {
            var response = new PagedResponse<MandateWithDetailListDto>();
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => (int?)m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
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
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
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
                        Success = false,
                        ErrorCode = 400,
                        Message = "The page number and page size must be greater than 1!"
                    };
                }
            }
            catch (Exception ex)
            {
                response.Error = new ErrorResponseDto()
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = ex.Message
                };
            }
            return response;
        }
        public async Task<List<MerchantMonthlySumDto>> GetMandateMonthlySum()
        {
            DateTime current = DateTime.Now;
            DateTime currentYear = DateTime.Parse($"{current.Year}/01/01");
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            var monthlyMandate = _context.zib_mandates.Where(m => m.CreatedDate >= currentYear).Where(m => m.MerchantId == merchantId)
                .GroupBy(o => new
                {
                    Month = o.CreatedDate.Value.Month
                })
                .Select(u => new MerchantMonthlySumDto
                {
                    ItemSum = u.Sum(x => x.Amount),
                    Month = u.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(u.Key.Month)
                })
                .ToList();
            return monthlyMandate;
        }
        public async Task<List<MerchantYearlySumDto>> GetFiveYearMandate()
        {
            var userName = _userResolverService.GetUserName();
            //var merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            var getUser = await _userManager.FindByNameAsync(userName);
            var loggedUserRole = await _userManager.GetRolesAsync(getUser);
            var loggedUserRoleName = loggedUserRole[0].ToString();
            long merchantId;
            if (loggedUserRoleName == "Merchant")
            {
                merchantId = _context.zib_merchants.Where(u => u.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            else
            {
                merchantId = _context.zib_merchant_users.Where(u => u.User.UserName == userName).Select(m => m.Id).FirstOrDefault();
            }
            var yearlyMandate = _context.zib_mandates.Where(m => m.CreatedDate > DateTime.Now.AddYears(-5)).Where(m => m.MerchantId == merchantId)
                .GroupBy(o => o.CreatedDate.Value.Year)
                .Select(u => new MerchantYearlySumDto
                {
                    ItemTotal = u.Sum(x => x.Amount),
                    Year = u.Key
                }
                ).ToList();

            //grand total
            var tot = yearlyMandate.Sum(s => s.ItemTotal);

            //apply percentage to each element
            yearlyMandate.ForEach(s => s.ItemPercent = Math.Round(((decimal)100.0 * (s.ItemTotal / tot)), 2));
            return yearlyMandate;
        }
    }
}
