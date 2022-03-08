﻿using CoreApi_BLL.Interfaces;
using CoreApi_EL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi_PL.Controllers
{
    //İşlemlerin sonucu nasıl geri dönmelidir?
    //OK 200 -->> Get işlemi
    //NoContent 204 -->> Güncelleme işlemi 
    //NoContent 204 -->> Silme işlemi 
    //Created 201 -->> Ekleme işlemi

    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAllAssignments()
        {
            try
            {
                var list = _unitOfWork.AssignmentRepository.GetAll().ToList();
                return Ok(list);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        //[HttpGet("[action]/{id}")]
        public IActionResult GetAssignmentById(int id)
        {
            try
            {
                var assignment = _unitOfWork.AssignmentRepository
                                    .GetFirstOrDefault(x => x.Id == id);
                if (assignment != null)
                {
                    return Ok(assignment);
                }
                return Problem();
            }
            catch (Exception)
            {
                return Problem();

            }
        }

        [HttpPost]
        public IActionResult AddAssignment(Assignment model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                bool result = _unitOfWork.AssignmentRepository.Add(model);
                if (result)
                {
                    return Created("", model);
                }
                return Problem();
            }
            catch (Exception)
            {

                return Problem();
            }
        }



        //api/Assignment/DeleteAssignment?id=2
        //[HttpGet("[action]")]
        //public IActionResult DeleteAssignment([FromQuery] int id)

        //api/Assignment/1 --> postman ile gönderirken HTTPDELETE seçiniz
        [HttpDelete("{id}")]

        //api/Assignment/DeleteAssignment/1 -->HTTPGET seçilmelidir
        //[HttpGet("[action]/{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            try
            {
                if (id > 0)
                {
                    var data = _unitOfWork.AssignmentRepository
                        .GetFirstOrDefault(x => x.Id == id);
                    if (data != null)
                    {
                        bool result = _unitOfWork.AssignmentRepository
                            .Delete(data);
                        if (result)
                        {
                            return NoContent();
                        }
                        return Problem();
                    }
                    return Problem();
                }
                return Problem();

            }
            catch (Exception)
            {

                return Problem();
            }
        }

        //api/Assignment/id
        [HttpPut("{id}")]
        public IActionResult UpdateAssignment(int id, Assignment model)
        {
            try
            {
                if (id > 0)
                {
                    var currentAssignment = _unitOfWork
                        .AssignmentRepository.GetFirstOrDefault(x => x.Id == id);
                    if (currentAssignment != null)
                    {
                        currentAssignment.Description = string.IsNullOrEmpty(model.Description) || model.Description == "XX" ? currentAssignment.Description : model.Description;
                        currentAssignment.IsCompleted = model.IsCompleted;
                        bool result = _unitOfWork.AssignmentRepository
                            .Update(currentAssignment);
                        if (result)
                        {
                            return NoContent();
                        }
                        throw new Exception("Görev bulundu ama beklenmedik bir hata nedeniyle güncelleştirme başarısızdır!");

                    }
                    throw new Exception("Görev bulunamadı! Gönderdiğiniz id bilgisini kontrol ediniz!");
                }
                throw new Exception("id sıfırıdan büyük olmalıdır!");
            }
            catch (Exception ex)
            {

                return Problem(ex.Message);
            }
        }
    }
}