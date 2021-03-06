﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // GET api/values
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [Route("/api/Login")]
        [HttpPost]
        public ActionResult Login([FromBody] Login value)
        {
            if(!string.IsNullOrEmpty(value.UserName)&& !string.IsNullOrEmpty(value.Password) && value.UserName=="Rajat"&& value.Password=="10#Hammer")
            {
                var claim = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,value.UserName)
                };
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(value.Password));
                int expiry=Convert.ToInt32(Configuration["Jwt:ExpiryInMinutes"]);
                var token = new JwtSecurityToken(
                    issuer: Configuration["Jwt:Site"],
                    audience: Configuration["Jwt:Site"],
                    expires: DateTime.Now.AddMinutes(330+expiry),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.DesEncryption));
                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
            }
            return Unauthorized();
            
        }
    }
}
