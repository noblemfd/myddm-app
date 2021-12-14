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
      //  private readonly UserResolverService _userResolverService;

        public AdminService(DDMDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
          //  _userResolverService = userResolverService;
        }

        public async Task<GenericResponseDto<AllMerchantListDto>> CreateMerchantAsync(MerchantCreateDto requestDto)
        {
            var existingMerchant = await _context.zib_merchants.FirstOrDefaultAsync(e => e.User.UserName == requestDto.UserName);
            var response = new GenericResponseDto<AllMerchantListDto>();

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
                    PasswordHash = "@SecretPassword123",
                };

                //  var result = await _userManager.CreateAsync(merchantUser, "@SecretPassword123");
                var result = await _userManager.CreateAsync(merchantUser);

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
                        merchant.User = merchantUser;
                      //  merchant.CreatedBy = userName;
                        _context.zib_merchants.Add(merchant);
                        await _context.SaveChangesAsync();

                        response.Result = _mapper.Map<AllMerchantListDto>(merchant);
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
                    var pagedMandates = await mandateQueryable.Include(l => l.Merchant)
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

            var mandate = await _context.zib_mandates.Include(l => l.Merchant)
                                                .ThenInclude(e => e.User)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (mandate != null)
            {
                response.Result = _mapper.Map<AllMandateListDto>(mandate);
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
    }
}
