using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.ElasticSearchCore.WebApi.Common;
using QICore.ElasticSearchCore.WebApi.Dao;
using QICore.ElasticSearchCore.WebApi.Models;
using QICore.ElasticSearchCore.WebApi.OptionModel;

namespace QICore.ElasticSearchCore.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ESController : ControllerBase
    {
        private readonly IOptions<ConnectionStringOption> _connStrings;
        private ITag _tag;
        private readonly ILogger<ESController> _logger;

        public ESController(ITag tag, IOptions<ConnectionStringOption> connStrings, ILogger<ESController> logger)
        {
            _tag = tag;
            _connStrings = connStrings;
            _logger = logger;
        }

        [HttpGet("{pageindex?}/{pagesize?}")]
        public OkObjectResult Get(int pageindex=1,int pagesize=10)
        {
            var list = _tag.GetTagList(pageindex, pagesize);
            return Ok(list);
        }

        [HttpPut("create_index/{indexName?}")]
        public OkObjectResult CreateIndex(string indexName="wizplant")
        {
            var client = ElasticSearchHelper.GetElasticClient(_connStrings.Value.ESWizplantUrl);
            var bol=ElasticSearchHelper.CreateIndex<ElasticModel>(client, indexName);            
            return Ok(new { name="创建索引",IsSuccess=bol});
        }
    }
}
