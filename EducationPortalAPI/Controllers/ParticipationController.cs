﻿using EducationPortalAPI.DAL;
using EducationPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationPortalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipationController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ParticipationController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Participations = await _appDbContext.Participations.ToListAsync();

                if (Participations.Any())
                    return Ok(Participations);
                else
                    return NotFound("Kayıt bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetParticipationById/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var Participation = await _appDbContext.Participations
                                    .Where(p => p.UserID == id)
                                    .ToListAsync();
                if (Participation is not null)
                    return Ok(Participation);
                else
                    return NotFound($"{id} id'li kayıt bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Participation model)
        {
            try
            {
                _appDbContext.Add(model);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kayıt eklendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Participation model)
        {
            if (model is null)
                return BadRequest("Eklenecek veri yok.");

            var Participation = await _appDbContext.Participations.FindAsync(model.ID);
            if (Participation is null)
                return NotFound($"{model.ID} id'li veri bulunmamaktadır.");

            Participation.CompletionStatus = model.CompletionStatus;
            Participation.RequestStatus = model.RequestStatus; 

            await _appDbContext.SaveChangesAsync();
            return Ok("Kullanıcı güncellendi");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequestModel requestModel)
        {
            var userId = requestModel.UserId;
            var educationId = requestModel.EducationId;

            var participationsToDelete = await _appDbContext.Participations
                                            .Where(p => p.UserID == userId && p.EducationID == educationId)
                                            .ToListAsync();

            if (participationsToDelete.Any())
            {
                _appDbContext.Participations.RemoveRange(participationsToDelete);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kayıt silindi");
            }
            else
            {
                return NotFound($"Kullanıcı {userId} ve eğitim {educationId} ile ilgili kayıt bulunmamaktadır.");
            }
        }

        public class DeleteRequestModel
        {
            public string UserId { get; set; }
            public int EducationId { get; set; }
        }

    }
}
