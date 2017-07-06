using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings;
using Lykke.Job.OffchainCashoutScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace Lykke.Job.OffchainCashoutScheduler.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IHubCashoutSettingsRepository _hubCashoutSettingsRepository;

        public SettingsController(IHubCashoutSettingsRepository hubCashoutSettingsRepository)
        {
            _hubCashoutSettingsRepository = hubCashoutSettingsRepository;
        }

        /// <summary>
        /// Get cashout settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(SettingsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var data = await _hubCashoutSettingsRepository.GetAll();

            return Ok(new SettingsModel
            {
                Settings = data.Select(x => new SettingValue { Asset = x.Key, Value = x.Value })
            });
        }

        /// <summary>
        /// Get cashout settings
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SettingsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Set([FromBody] SettingsModel model)
        {
            await _hubCashoutSettingsRepository.SetAll(model.Settings.ToDictionary(x => x.Asset, x => x.Value));

            return Ok();
        }
    }
}
