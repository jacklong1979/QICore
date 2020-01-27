using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.ElasticSearchCore.WebApi.Dao;
using QICore.ElasticSearchCore.WebApi.OptionModel;

namespace QICore.ElasticSearchCore.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ESController : ControllerBase
    {
        private ITag _tag;
        private readonly ILogger<ESController> _logger;

        public ESController(ITag tag,ILogger<ESController> logger)
        {
            _tag = tag;
            _logger = logger;
        }

        [HttpGet("{pageindex?}/{pagesize?}")]
        public OkObjectResult Get(int pageindex=1,int pagesize=10)
        {
            var list = _tag.GetTagList(pageindex, pagesize);
            return Ok(list);
        }       
    }
}
