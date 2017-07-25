using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rates.Web.Dto;
using Rates.Web.Services;

namespace Rates.Web.Controllers
{
    [Route("api/[controller]")]
    public class RatesController : Controller
    {
        private readonly IRatesService _ratesService;

        public RatesController(IRatesService ratesService)
        {
            _ratesService = ratesService;
        }
        
        [HttpGet]
        public RatesDto Get()
        {
            return _ratesService.GetRates();
        }
    }
}
